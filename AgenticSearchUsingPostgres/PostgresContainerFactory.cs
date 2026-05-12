using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace AgenticSearchUsingPostgres;

public static class PostgresContainerFactory
{
    public static async Task<PostgreSqlContainer> GetPostgresContainerAsync()
    {
        var postgresContainer = new PostgreSqlBuilder("pgvector/pgvector:pg16")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .WithReuse(true)
            .Build();

        await postgresContainer.StartAsync();

        var dbContext = postgresContainer.GetDbContext();

        Console.WriteLine($"Database started at: {postgresContainer.GetConnectionString()}");

        await dbContext.Database.ExecuteSqlAsync($"CREATE EXTENSION IF NOT EXISTS vector");
        await dbContext.Database.EnsureCreatedAsync();

        await LoadSampleDataIfNeeded(dbContext);

        return postgresContainer;
    }

    private static async Task LoadSampleDataIfNeeded(VectorDbContext dbContext)
    {
        const string dataFile = "data.sql";

        if (!File.Exists(dataFile))
        {
            return;
        }

        var count = await dbContext.Vectors.CountAsync();
        if (count > 0)
        {
            return;
        }

        Console.WriteLine("Loading sample data from data.sql...");

        var sql = await File.ReadAllTextAsync(dataFile);
        await dbContext.Database.ExecuteSqlRawAsync(sql);

        Console.WriteLine($"Loaded {await dbContext.Vectors.CountAsync()} sample records");
    }

    public static VectorDbContext GetDbContext(this PostgreSqlContainer postgresContainer)
    {
        return new VectorDbContext(postgresContainer.GetConnectionString());
    }
}
