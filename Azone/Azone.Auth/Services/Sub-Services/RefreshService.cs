using Azone.Auth.Services.SubServices;
using Azone.Shared.Cache;
using Azone.Shared.Models;
using Azone.Shared.Models.Utils;
using TokenPair = Azone.Models.Generated.TokenPair;

namespace Azone.Auth.Services;

public class RefreshService
{
    private readonly ILogger<RefreshService> _logger;
    private readonly IRedisCacheService _cache;
    private readonly IJwtService _jwtService;
    
    public RefreshService(IRedisCacheService cache, ILogger<RefreshService> logger, IJwtService jwtService)
    {
        _logger = logger;
        _cache = cache;
        _jwtService = jwtService;
    }

    public async Task<TokenPair> CreateTokenPair(User user)
    {
        var jwt = _jwtService.IssueToken(user.Id, new[] { user.UserRole.ToString() });
        var refresh = Guid.NewGuid().ToString();
        var pair = new TokenPair
        {
            AccessToken = jwt,
            RefreshToken = refresh
        };

        var payload = new RefreshTokenPayload
        {
            UserId = user.Id,
            Role = user.UserRole
        };

        var userRefreshKey = $"user:refresh:{user.Id}";
    
        var oldRefresh = await _cache.GetAsync<string>(userRefreshKey);
    
        if (!string.IsNullOrEmpty(oldRefresh))
        {
            await _cache.RemoveAsync($"refresh:{oldRefresh}");
        }

        await _cache.SetAsync($"refresh:{refresh}", payload, TimeSpan.FromDays(7));
    
        await _cache.SetAsync(userRefreshKey, refresh, TimeSpan.FromDays(7));

        return pair;
    }
    
    public async Task<TokenPair> CreateTokenPair(RefreshTokenPayload payload)
    {
        var jwt = _jwtService.IssueToken(payload.UserId, new[] { payload.Role.ToString() });
        var refresh = Guid.NewGuid().ToString();
        var pair = new TokenPair
        {
            AccessToken = jwt,
            RefreshToken = refresh
        };

        var userRefreshKey = $"user:refresh:{payload.UserId}";
        var oldRefresh = await _cache.GetAsync<string>(userRefreshKey);
    
        if (!string.IsNullOrEmpty(oldRefresh))
        {
            await _cache.RemoveAsync($"refresh:{oldRefresh}");
        }

        await _cache.SetAsync($"refresh:{refresh}", payload, TimeSpan.FromDays(7));
        await _cache.SetAsync(userRefreshKey, refresh, TimeSpan.FromDays(7));

        return pair;
    }

    public async Task<Result<TokenPair>> RefreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result<TokenPair>.Failure("Refresh token is required");

        var refreshKey = $"refresh:{refreshToken}";
        var payload = await _cache.GetAsync<RefreshTokenPayload>(refreshKey);
    
        if (payload == null)
            return Result<TokenPair>.Failure("Refresh token is invalid or expired");

        await _cache.RemoveAsync(refreshKey);
        await _cache.RemoveAsync($"user:refresh:{payload.UserId}");

        var pair = await CreateTokenPair(payload);
        return Result<TokenPair>.Success(pair);
    }
}