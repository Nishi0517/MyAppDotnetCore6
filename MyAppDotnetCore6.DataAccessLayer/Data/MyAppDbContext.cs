using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAppDotnetCore6.Models;

namespace MyAppWebCore6.DataAccessLayer
{
    public class MyAppDbContext:IdentityDbContext//DbContext
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options):base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}