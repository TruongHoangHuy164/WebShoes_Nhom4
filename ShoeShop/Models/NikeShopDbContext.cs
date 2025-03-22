using Microsoft.EntityFrameworkCore;

namespace ShoeShop.Models
{
    public class NikeShopDbContext : DbContext
    {
        public NikeShopDbContext(DbContextOptions<NikeShopDbContext> options) : base(options) { }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }


        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
       // public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Size> Sizes { get; set; }
       // public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        // public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
