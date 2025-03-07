using Microsoft.EntityFrameworkCore;

namespace IntroToSemanticKernel;

public class RestaurantDbContext : DbContext
{
    private readonly string _connectionString;

    public RestaurantDbContext(string connectionString) => _connectionString = connectionString;

    public DbSet<RestaurantBooking> RestaurantBookings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
    }
}