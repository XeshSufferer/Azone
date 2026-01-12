using Azone.Merchant.Models;
using Azone.Merchant.Models.Records;
using Microsoft.EntityFrameworkCore;

public class MerchantDbContext : DbContext
{
    
    public MerchantDbContext(DbContextOptions options) : base(options) { }
    
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<ShopOwner> ShopOwners => Set<ShopOwner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shop>(builder =>
        {
            builder.HasKey(s => s.Id);

            builder
                .HasMany<ShopOwner>()
                .WithOne()
                .HasForeignKey(x => x.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Navigation(s => s.Owners)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });


        modelBuilder.Entity<ShopOwner>(builder =>
        {
            builder.HasKey(x => new { x.ShopId, x.UserId }); // composite key
        });
    }
}