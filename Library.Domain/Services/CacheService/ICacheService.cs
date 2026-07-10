namespace Library.Domain.Services.CacheService;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? absoluteExpiry = null);
    Task RemoveAsync(string key);
    Task<long> IncrementAsync(string key);
}