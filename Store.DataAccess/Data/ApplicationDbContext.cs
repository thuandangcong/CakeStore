
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "TempData",
                    StreetAddress = "123 Tech St",
                    City = "TempData City",
                    PostalCode = "12121",
                    State = "IL",
                    PhoneNumber = "6669990000"
                }
                );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "TempDataCategory", DisplayOrder = 1 }

                );
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "TempDataBrand", DisplayOrder = 1 }

                );
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "TempDataProductName", Description = "TempDataDescription", Price = 454.45, CategoryId = 1, BrandId = 1 }
                );
        }
    }
}
