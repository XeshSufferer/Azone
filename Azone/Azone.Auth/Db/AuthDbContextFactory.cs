using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Azone.Auth.Db;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=main;Username=postgres;Password=dev"
        );
        return new AuthDbContext(optionsBuilder.Options);
    }
}