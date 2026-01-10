using Azone.Shared.Models.Enums;

namespace Azone.Shared.Models;

public class RefreshTokenPayload
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}