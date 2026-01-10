using Azone.Infra.Contracts.Enums;

namespace Azone.Auth.Models;

public class RefreshTokenPayload
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}