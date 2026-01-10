using Azone.Auth.Helpers;
using Azone.Auth.Services.SubServices;
using Azone.Models.Generated;
using Azone.Shared.DBs;
using Azone.Shared.Models;
using Azone.Shared.Models.Enums;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TokenPair = Azone.Models.Generated.TokenPair;

namespace Azone.Auth.Services;

public class AuthService(ILogger<AuthService> logger, AppDbContext db,
    IHasher hasher, IRefreshService refreshService) : Models.Generated.Auth.AuthBase
{
    public override async Task<CreateAccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context)
    {
        var user = new User()
        {
            Login = request.Login,
            PasswordHash = hasher.HashBcrypt(request.Password),
            UserRole = UserRole.User
        };
        
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        
        var reply = new CreateAccountReply
        {
            Tokens = await refreshService.CreateTokenPair(user)
        };
        
        return reply;
    }

    public override async Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        var findedUser = await db.Users.Where(u => u.Login == request.Login).FirstOrDefaultAsync();
        if (findedUser == null)
        {
            
        }
        
        var reply = new LoginReply
        {
            Tokens = await refreshService.CreateTokenPair(findedUser)
        }
    }

    public override Task<LogoutReply> Logout(LogoutRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LogoutReply());
    }

    public override Task<TokenPair> Refresh(RefreshToken request, ServerCallContext context)
    {
        return Task.FromResult(new TokenPair());
    }

    public override Task<TokenIsValid> TokenValidate(RefreshToken request, ServerCallContext context)
    {
        return Task.FromResult(new TokenIsValid()
        {
            IsValid = true
        });
    }
}