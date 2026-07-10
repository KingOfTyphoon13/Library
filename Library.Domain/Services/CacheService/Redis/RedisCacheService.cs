using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Library.Domain.Services.CacheService.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly RedisCacheSettings _settings;

    public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisCacheSettings> settings)
    {
        _db = redis.GetDatabase();
        _settings = settings.Value;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty) return default;

        var stringValue = (string)value!;
        return JsonSerializer.Deserialize<T>(stringValue);
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null)
    {
        var options = new DistributedCacheEntryOptions();
        var expiry = absoluteExpiry ?? _settings.AbsoluteExpiration;

        return await _db.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<long> IncrementAsync(string key)
    {
        return await _db.StringIncrementAsync(key);
    }
}
