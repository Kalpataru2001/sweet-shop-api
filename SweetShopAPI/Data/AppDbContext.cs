using Microsoft.EntityFrameworkCore;
using SweetShopAPI.Models;

namespace SweetShopAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // This links your C# class to the "Sweets" table in Supabase
        public DbSet<Sweet> Sweets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
    }
}
