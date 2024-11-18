using DynamicFilterAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicFilterAPI.Databases;

public class ProductDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(new List<Product>()
        {
            new Product() { Id = 1, Category = "TV", isActive = true, Name = "LG", Price = 500 },
            new Product() { Id = 2, Category = "TV", isActive = true, Name = "Sony", Price = 1500 },
            new Product() { Id = 3, Category = "TV", isActive = true, Name = "ABC", Price = 2500 },
            new Product() { Id = 4, Category = "TV", isActive = false, Name = "ABC2", Price = 2500 },
        });
        base.OnModelCreating(modelBuilder);
    }
}