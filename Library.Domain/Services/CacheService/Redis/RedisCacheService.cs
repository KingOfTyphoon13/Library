using Library.Domain.Services.CacheService.Keys;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Library.Domain.Services.CacheService.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly RedisCacheSettings _settings;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisCacheSettings> settings, ILogger<RedisCacheService> logger)
    {
        _db = redis.GetDatabase();
        _settings = settings.Value;

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<T?> GetAsync<T>(CacheKey key)
    {
        var resolvedKey = await ResolveAsync(key);
        var value = await _db.StringGetAsync(resolvedKey);

        if (value.IsNullOrEmpty)
        {
            _logger.LogDebug("Cache miss for key {Key}", resolvedKey);
            return default;
        }

        _logger.LogDebug("Cache hit for key {Key}", resolvedKey);
        return JsonSerializer.Deserialize<T>((string)value!);
    }

    public async Task SetAsync<T>(CacheKey key, T value, TimeSpan? absoluteExpiry = null)
    {
        var resolvedKey = await ResolveAsync(key);
        var expiry = absoluteExpiry ?? _settings.AbsoluteExpiration;

        var success = await _db.StringSetAsync(resolvedKey, JsonSerializer.Serialize(value), expiry);
        _logger.LogDebug("Cache set for key {Key}, success: {Success}", resolvedKey, success);
    }

    public async Task RemoveAsync(CacheKey key)
    {
        var resolvedKey = await ResolveAsync(key);
        await _db.KeyDeleteAsync(resolvedKey);
        _logger.LogDebug("Cache entry removed for key {Key}", resolvedKey);
    }

    public async Task<long> InvalidateAsync(string resource)
    {
        var newVersion = await _db.StringIncrementAsync($"{resource}:version");
        _logger.LogInformation("Cache invalidated for resource {Resource}, new version: {Version}", resource, newVersion);
        return newVersion;
    }

    private async Task<string> ResolveAsync(CacheKey key)
    {
        if (!key.IsVersioned) return $"{key.Resource}:{key.Suffix}";

        var version = await _db.StringGetAsync($"{key.Resource}:version");
        var v = version.IsNullOrEmpty ? 0 : (long)version;
        return $"{key.Resource}:v{v}:{key.Suffix}";
    }
}