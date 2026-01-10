using Azone.Accounts.Services.Models;
using Azone.Accounts.Services.Sub_Services.Contracts;
using Azone.Auth.Helpers;
using Azone.Contracts.Models.Generated;
using Azone.Infra.Contracts.Enums;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Azone.Accounts.Services;

public class AuthService(ILogger<AuthService> logger, AuthDbContext db,
    IHasher hasher, IRefreshService refreshService) : Contracts.Models.Generated.Auth.AuthBase
{
    public override async Task<CreateAccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context)
    {
        if (await db.Users.AnyAsync(u => u.Login == request.Login))
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "User with this login already exists"));
        }
        
        var user = new User
        {
            Login = request.Login,
            PasswordHash = hasher.HashBcrypt(request.Password),
            Role = UserRole.User
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
            throw new RpcException(new Status(StatusCode.NotFound, "Login or password incorrect"));

        if(!hasher.Verify(request.Password, findedUser.PasswordHash))
            throw new RpcException(new Status(StatusCode.NotFound, "Login or password incorrect"));
        
        var reply = new LoginReply
        {
            Tokens = await refreshService.CreateTokenPair(findedUser)
        };
        
        return reply;
    }

    public override async Task<LogoutReply> Logout(LogoutRequest request, ServerCallContext context)
    {
        var result = await refreshService.KillRefreshToken(request.RefreshToken);
        
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Refresh token invalid"));

        return new LogoutReply { IsSuccess = result.IsSuccess };;
    }

    public override async Task<TokenPair> Refresh(RefreshToken request, ServerCallContext context)
    {
        var result = await refreshService.RefreshToken(request.Refresh);
        
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));

        return result.Value;
    }
}