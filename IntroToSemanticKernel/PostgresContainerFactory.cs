using IntroToSemanticKernel;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace VectorSearchUsingPostgres;

public static class PostgresContainerFactory
{
    public static async Task<PostgreSqlContainer> GetPostgresContainerAsync()
    {
        var postgresContainer = new PostgreSqlBuilder()
            .WithImage("pgvector/pgvector:pg16")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithCleanUp(true)
            .Build();

        await postgresContainer.StartAsync();
        
        var dbContext = postgresContainer.GetDbContext();
        
        string connectionString = postgresContainer.GetConnectionString();

        Console.WriteLine($"Database started at: {connectionString}");

        // Apply migrations
        await dbContext.Database.EnsureCreatedAsync();
        
        return postgresContainer;
    }

    public static RestaurantDbContext GetDbContext(this PostgreSqlContainer postgresContainer)
    {
        string connectionString = postgresContainer.GetConnectionString();

        return new RestaurantDbContext(connectionString);
    }
}