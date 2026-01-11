using Azone.Auth.Db;
using Azone.Auth.Mappers;
using Azone.Auth.Utils;
using Azone.Auth.Helpers;
using Azone.Auth.Models;
using Azone.Auth.Services.Sub_Services.Contracts;
using Azone.Contracts.Models.Generated;
using Azone.Infra.Contracts.Enums;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Azone.Auth.Services;

public class AuthService(ILogger<AuthService> logger, AuthDbContext db,
    IHasher hasher, IRefreshService refreshService) : Contracts.Models.Generated.Auth.AuthBase
{
    public override async Task<CreateAccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context)
    {
        if (await db.Users.AnyAsync(u => u.Login == request.Login))
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, AuthError.UserWithLoginAlreadyExist.Code()));
        }
        
        var user = User.Create(request.Login, request.Password, hasher);
        
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
            throw new RpcException(new Status(StatusCode.NotFound, "Auth.LoginOrPasswordIncorrect"));

        if(!hasher.Verify(request.Password, findedUser.PasswordHash))
            throw new RpcException(new Status(StatusCode.NotFound, "Auth.LoginOrPasswordIncorrect"));
        
        var reply = new LoginReply
        {
            Tokens = await refreshService.CreateTokenPair(findedUser)
        };
        
        return reply;
    }

    public override async Task<IsSuccess> Logout(RefreshToken request, ServerCallContext context)
    {
        var result = await refreshService.KillRefreshToken(request.Refresh);
        
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Auth.LoginOrPasswordIncorrect"));

        return new IsSuccess { Success = true };;
    }

    public override async Task<TokenPair> Refresh(RefreshToken request, ServerCallContext context)
    {
        var result = await refreshService.RefreshToken(request.Refresh);
        
        if(!result.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, result.Error));

        return result.Value;
    }

    public override async Task<IsSuccess> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, AuthError.InvalidPasswordOrLogin.Code()));

        var resut = user.ChangePassword(request.Password, request.NewPassword, hasher);

        if(!resut.IsSuccess)
            throw new RpcException(new Status(StatusCode.InvalidArgument, resut.Error));

        await db.SaveChangesAsync();
        
        return new IsSuccess { Success = true };
    }

    public override async Task<IsSuccess> UserExist(UserId request, ServerCallContext context)
    {
        return new IsSuccess{Success = await db.Users.AnyAsync(u => u.Id == request.Id)};
    }

    public override async Task<UserData> GetUserData(UserId request, ServerCallContext context)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == request.Id);
        
        if(user == null)
            throw new RpcException(new Status(StatusCode.NotFound, AuthError.UserNotFound.Code()));

        return user.ToUserData();
    }
}