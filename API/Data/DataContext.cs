using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        //public DbSet<Product> Products => Set<Product>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new List<Product>
            {
                new Product {Id=1, Name="Iphone 12", Description="Siyah",Price=12000,Stock=10,ImageUrl="1.jpg",IsActive=true},
                new Product {Id=2, Name="Iphone 13", Description="Siyah",Price=13000,Stock=10,ImageUrl="2.jpg",IsActive=true},
                new Product {Id=3, Name="Iphone 14", Description="Siyah",Price=14000,Stock=10,ImageUrl="3.jpg",IsActive=true},
                new Product {Id=4, Name="Iphone 15", Description="Siyah",Price=15000,Stock=10,ImageUrl="4.jpg",IsActive=true},
                new Product {Id=5, Name="Iphone 16", Description="Siyah",Price=16000,Stock=10,ImageUrl="5.jpg",IsActive=true},
            });
        }
    }
}
