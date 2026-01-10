using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Azone.Auth.Db;

public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        
        optionsBuilder.UseNpgsql("Server=(localdb)\\mssqllocaldb;Database=AzoneAuthDb;Trusted_Connection=True;");

        return new AuthDbContext(optionsBuilder.Options);
    }
}