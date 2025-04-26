using Microsoft.EntityFrameworkCore;
using OrderProcessApi.OrderProcessing.Models;

namespace OrderProcessApi.OrderProcessing.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed initial data
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, ProductName = "Laptop", Price = 999.99m },
            new Product { ProductId = 2, ProductName = "Smartphone", Price = 699.99m },
            new Product { ProductId = 3, ProductName = "Headphones", Price = 149.99m },
            new Product { ProductId = 4, ProductName = "Keyboard", Price = 49.99m },
            new Product { ProductId = 5, ProductName = "Mouse", Price = 29.99m }
        );
    }
}