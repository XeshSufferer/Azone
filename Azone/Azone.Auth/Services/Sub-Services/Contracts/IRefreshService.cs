using Azone.Shared.Models;
using Azone.Shared.Models.Utils;
using TokenPair = Azone.Models.Generated.TokenPair;

namespace Azone.Auth.Services;

public interface IRefreshService
{
    Task<TokenPair> CreateTokenPair(User user);

    Task<TokenPair> CreateTokenPair(RefreshTokenPayload payload);

    Task<Result<TokenPair>> RefreshToken(string refreshToken);
}