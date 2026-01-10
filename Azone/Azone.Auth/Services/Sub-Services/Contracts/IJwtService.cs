namespace Azone.Accounts.Services.Sub_Services.Contracts;

public interface IJwtService
{
    string IssueToken(int userId, IEnumerable<string>? roles = null);
    
}