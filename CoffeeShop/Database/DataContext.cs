using Microsoft.EntityFrameworkCore;
using CoffeeShop.Model;

namespace CoffeeShop.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Storages> Storages { get; set; }
        public DbSet<Customers> Customers { get; set; }

        public DbSet<Employees> Employees { get; set; }

        public DbSet<Categories> Categories { get; set; }

        public DbSet<Products> Products { get; set; }

        public DbSet<Orders> Orders { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
