using Microsoft.EntityFrameworkCore;

namespace AgenticSearchUsingPostgres;

public class VectorDbContext : DbContext
{
    private readonly string _connectionString;

    public VectorDbContext(string connectionString) => _connectionString = connectionString;

    public DbSet<VectorEntity> Vectors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<VectorEntity>(entity =>
        {
            entity.Property(e => e.SearchVector)
                .HasComputedColumnSql(
                    "to_tsvector('english', coalesce(\"Name\", '') || ' ' || coalesce(\"Contents\", ''))",
                    stored: true);

            entity.HasIndex(e => e.SearchVector)
                .HasMethod("GIN");

            entity.HasIndex(e => e.Embedding)
                .HasMethod("hnsw")
                .HasOperators("vector_cosine_ops");
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString, o => o.UseVector());
    }
}
