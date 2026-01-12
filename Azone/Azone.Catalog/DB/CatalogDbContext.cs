using Azone.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Azone.Catalog.DB;

public class CatalogDbContext : DbContext
{
    public  CatalogDbContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(builder =>
        {
            builder.HasKey(s => s.Id);

            builder
                .HasMany<LogoUrl>()
                .WithOne()
                .HasForeignKey(x => x.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Navigation(s => s.ImageUrls)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder.HasIndex(s => s.CreatedAt)
                .IsDescending()
                .HasDatabaseName("IX_Products_CreatedAt_DESC");
            
            builder.HasIndex(s => new { s.ShopId, s.CreatedAt })
                .IsDescending();
                
        });
    }
}