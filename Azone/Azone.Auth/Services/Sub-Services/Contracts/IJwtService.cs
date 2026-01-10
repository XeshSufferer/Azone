namespace Azone.Auth.Services.SubServices;

public interface IJwtService
{
    string IssueToken(int userId, IEnumerable<string>? roles = null);
    
}