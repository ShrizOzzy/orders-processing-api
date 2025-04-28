using Microsoft.EntityFrameworkCore;
using OrderProcess.Core.Models;

namespace OrderProcess.Data.DataExtentions;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}