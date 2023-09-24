using Microsoft.EntityFrameworkCore;

namespace AuthApi.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=blogdb;Username=postgres;Password=8085");
    }
}