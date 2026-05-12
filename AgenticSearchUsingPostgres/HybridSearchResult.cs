namespace AgenticSearchUsingPostgres;

public class HybridSearchResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Contents { get; set; } = string.Empty;
    public double RrfScore { get; set; }
    public double VectorDistance { get; set; }
    public long? VectorRank { get; set; }
    public double? TextRankScore { get; set; }
    public long? TextRank { get; set; }
}
