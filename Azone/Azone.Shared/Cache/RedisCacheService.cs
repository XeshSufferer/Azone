namespace Azone.Shared.Cache;

using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly JsonSerializerOptions _serializerOptions;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (!value.HasValue)
            return default;

        return JsonSerializer.Deserialize<T>(value.ToString(), _serializerOptions);
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var serializedValue = JsonSerializer.Serialize(value, _serializerOptions);
        return await _database.StringSetAsync(key, serializedValue, expiration, When.Always, CommandFlags.None);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _database.KeyExistsAsync(key);
    }
}