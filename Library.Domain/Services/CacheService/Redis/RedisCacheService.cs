using Library.Domain.Services.CacheService.Keys;
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
    public async Task<T?> GetAsync<T>(CacheKey key)
    {
        var value = await _db.StringGetAsync(await ResolveAsync(key));
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SetAsync<T>(CacheKey key, T value, TimeSpan? absoluteExpiry = null)
    {
        var expiry = absoluteExpiry ?? _settings.AbsoluteExpiration;
        await _db.StringSetAsync(await ResolveAsync(key), JsonSerializer.Serialize(value), expiry);
    }

    public async Task RemoveAsync(CacheKey key) => await _db.KeyDeleteAsync(await ResolveAsync(key));

    public Task InvalidateAsync(string resource) => _db.StringIncrementAsync($"{resource}:version");

    private async Task<string> ResolveAsync(CacheKey key)
    {
        if (!key.IsVersioned) return $"{key.Resource}:{key.Suffix}";

        var version = await _db.StringGetAsync($"{key.Resource}:version");
        var v = version.IsNullOrEmpty ? 0 : (long)version;
        return $"{key.Resource}:v{v}:{key.Suffix}";
    }
}
