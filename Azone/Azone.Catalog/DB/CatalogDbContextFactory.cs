using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Azone.Catalog.DB;

public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
        
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=main;Username=postgres;Password=dev"
        );
        return new CatalogDbContext(optionsBuilder.Options);
    }
}