using Azone.Infra.Contracts.Enums;

namespace Azone.Accounts.Services.Models;

public class RefreshTokenPayload
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}