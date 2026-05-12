using Microsoft.EntityFrameworkCore;
using OpenAI.Embeddings;
using Pgvector;

namespace AgenticSearchUsingPostgres;

public class HybridSearchService
{
    private readonly VectorDbContext _dbContext;
    private readonly EmbeddingClient _embeddingClient;
    private const int K = 60; // RRF constant
    private const int DefaultLimit = 10;

    public HybridSearchService(VectorDbContext dbContext, EmbeddingClient embeddingClient)
    {
        _dbContext = dbContext;
        _embeddingClient = embeddingClient;
    }

    public async Task<List<HybridSearchResult>> SearchAsync(
        string queryText,
        int limit = DefaultLimit,
        double vectorWeight = 1.0,
        double textWeight = 1.0)
    {
        var embedding = await _embeddingClient.GenerateEmbeddingAsync(queryText);
        var queryVector = new Vector(embedding.Value.ToFloats());

        var query = $@"
WITH vector_search AS (
    SELECT
        ""Id"",
        ""Embedding"" <=> {{0}}::vector AS distance,
        ROW_NUMBER() OVER (ORDER BY ""Embedding"" <=> {{0}}::vector) AS rank
    FROM ""Vectors""
),
text_search AS (
    SELECT
        ""Id"",
        ts_rank(""SearchVector"", websearch_to_tsquery('english', {{1}})) AS rank_score,
        ROW_NUMBER() OVER (ORDER BY ts_rank(""SearchVector"", websearch_to_tsquery('english', {{1}})) DESC) AS rank
    FROM ""Vectors""
    WHERE ""SearchVector"" @@ websearch_to_tsquery('english', {{1}})
),
combined AS (
    SELECT
        v.""Id"",
        COALESCE({vectorWeight} * 1.0 / ({K} + vs.rank), 0) + COALESCE({textWeight} * 1.0 / ({K} + ts.rank), 0) AS rrf_score,
        vs.distance AS vector_distance,
        vs.rank AS vector_rank,
        ts.rank_score AS text_rank_score,
        ts.rank AS text_rank
    FROM ""Vectors"" v
    LEFT JOIN vector_search vs ON v.""Id"" = vs.""Id""
    LEFT JOIN text_search ts ON v.""Id"" = ts.""Id""
    WHERE vs.""Id"" IS NOT NULL OR ts.""Id"" IS NOT NULL
)
SELECT
    v.""Id"" AS ""Id"",
    v.""Name"" AS ""Name"",
    v.""Contents"" AS ""Contents"",
    c.rrf_score AS ""RrfScore"",
    c.vector_distance AS ""VectorDistance"",
    c.vector_rank AS ""VectorRank"",
    c.text_rank_score AS ""TextRankScore"",
    c.text_rank AS ""TextRank""
FROM combined c
JOIN ""Vectors"" v ON c.""Id"" = v.""Id""
ORDER BY c.rrf_score DESC
LIMIT {limit}";

        return await _dbContext.Database.SqlQueryRaw<HybridSearchResult>(
            query,
            queryVector.ToArray(),
            queryText
        ).ToListAsync();
    }

    public async Task<VectorEntity?> GetProductByIdAsync(int id)
    {
        return await _dbContext.Vectors.FindAsync(id);
    }

    public async Task<List<VectorEntity>> SearchByCategoryAsync(string category, int limit = 10)
    {
        return await _dbContext.Database.SqlQueryRaw<VectorEntity>(
            @"SELECT ""Id"", ""Name"", ""Contents"", ""Embedding"", ""SearchVector""
              FROM ""Vectors""
              WHERE ""SearchVector"" @@ websearch_to_tsquery('english', {0})
              ORDER BY ts_rank(""SearchVector"", websearch_to_tsquery('english', {0})) DESC
              LIMIT {1}",
            category,
            limit
        ).ToListAsync();
    }
}
