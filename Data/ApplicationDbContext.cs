using MarketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketingAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique(); // Ensure login username and password are unique
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique(); // Ensure email is unique.
        });
    }
}