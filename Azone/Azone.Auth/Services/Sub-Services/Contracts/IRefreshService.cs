using Azone.Auth.Models;
using Azone.Contracts.Models.Generated;
using Azone.Infra.Common.Models;

namespace Azone.Auth.Services.Sub_Services.Contracts;

public interface IRefreshService
{
    Task<TokenPair> CreateTokenPair(User user);

    Task<TokenPair> CreateTokenPair(RefreshTokenPayload payload);

    Task<Result<TokenPair>> RefreshToken(string refreshToken);

    Task<Result> KillRefreshToken(string refreshToken);
}