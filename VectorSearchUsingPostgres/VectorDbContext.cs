using Microsoft.EntityFrameworkCore;

namespace VectorSearchUsingPostgres;

public class VectorDbContext : DbContext
{
    private readonly string _connectionString;

    public VectorDbContext(string connectionString) => _connectionString = connectionString;

    public DbSet<VectorEntity> Vectors { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString, o =>
        {
            o.UseVector();
        });
    }
}