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
            builder.HasKey(p => p.Id);

            builder.OwnsMany(
                p => p.ImageUrls,
                ownedBuilder =>
                {
                    ownedBuilder.Property(l => l.Url).IsRequired().HasMaxLength(2048);
                    ownedBuilder.Property(l => l.ShopId).IsRequired();

                });

            builder.HasIndex(p => p.CreatedAt).IsDescending();
            builder.HasIndex(p => new { p.ShopId, p.CreatedAt }).IsDescending();
        });
    }
}