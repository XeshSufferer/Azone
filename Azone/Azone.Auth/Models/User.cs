using Azone.Auth.Utils;
using Azone.Auth.Helpers;
using Azone.Auth.Services;
using Azone.Infra.Common.Models;
using Azone.Infra.Contracts.Enums;

namespace Azone.Auth.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }

    public static User Create(string login, string password, IHasher hasher)
    {
        return new User
        {
            Login = login,
            PasswordHash = hasher.Hash(password),
            Role = UserRole.User
        };
    }

    public Result ChangePassword(string oldPassword, string newPassword, IHasher hasher)
    {
        if(!hasher.Verify(oldPassword, PasswordHash))
            return Result.Failure(AuthError.InvalidPasswordOrLogin.Code());
        
        PasswordHash = hasher.Hash(newPassword);
        return Result.Success();
    }
}