using Library.Domain.Services.CacheService.Keys;

namespace Library.Domain.Services.CacheService;

public interface ICacheService
{
    Task<T?> GetAsync<T>(CacheKey key);
    Task SetAsync<T>(CacheKey key, T value, TimeSpan? absoluteExpiry = null);
    Task RemoveAsync(CacheKey key);
    Task InvalidateAsync(string resource);
}