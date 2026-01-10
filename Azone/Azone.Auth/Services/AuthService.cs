using Azone.Models.Generated;
using Grpc.Core;

namespace Azone.Accounts.Services;

public class AuthService(ILogger<AuthService> logger) : Auth.AuthBase
{
    public override Task<CreateAccountReply> CreateAccount(CreateAccountRequest request, ServerCallContext context)
    {
        return Task.FromResult(new CreateAccountReply());
    }

    public override Task<LoginReply> Login(LoginRequest request, ServerCallContext context)
    {
        return Task.FromResult(new LoginReply());
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