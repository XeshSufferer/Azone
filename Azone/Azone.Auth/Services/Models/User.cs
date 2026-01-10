using Azone.Infra.Contracts.Enums;

namespace Azone.Accounts.Services.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    
}