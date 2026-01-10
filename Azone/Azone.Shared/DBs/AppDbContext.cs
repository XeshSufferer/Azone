using Azone.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Azone.Shared.DBs;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Item> Products { get; set; }
    public DbSet<Shop> Shops { get; set; }
    public DbSet<ProductReview> ProductReviews { get; set; }
    public DbSet<ShopReview> ShopReviews { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<CartItem>()
            .HasIndex(c => c.UserId);

        modelBuilder.Entity<Item>()
            .HasIndex(p => p.ShopId);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Shops)
            .WithMany(s => s.Admins);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Item>()
            .HasMany(p => p.ProductReviews)
            .WithOne(p => p.Item)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Shop>()
            .HasMany(s => s.Products)
            .WithOne(s => s.Shop)
            .HasForeignKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Shop>()
            .HasMany(s => s.Reviews)
            .WithOne(s => s.Shop)
            .HasForeignKey(s => s.ShopId)
            .OnDelete(DeleteBehavior.Cascade);;
    }
}