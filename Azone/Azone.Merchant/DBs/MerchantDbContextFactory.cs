using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Azone.Merchant.DBs;

public class MerchantDbContextFactory : IDesignTimeDbContextFactory<MerchantDbContext> 
{
    public MerchantDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MerchantDbContext>();
        
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=main;Username=postgres;Password=dev"
        );
        return new MerchantDbContext(optionsBuilder.Options);
    }
}