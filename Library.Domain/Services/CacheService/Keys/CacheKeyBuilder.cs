namespace Library.Domain.Services.CacheService.Keys;

internal sealed class CacheKeyBuilder
{
    private readonly string _resource;
    internal CacheKeyBuilder(string resource) => _resource = resource;

    internal CacheKey Paged(int pageNumber, int pageSize) =>
        CacheKey.Versioned(_resource, $"paged:{pageNumber}:{pageSize}");

    internal CacheKey Keyset(int? lastItemId, int pageSize) =>
        CacheKey.Versioned(_resource, $"keyset:{lastItemId?.ToString() ?? "start"}:{pageSize}");

    internal CacheKey PagedScoped(string scopeName, int scopeId, int pageNumber, int pageSize) =>
        CacheKey.Versioned(_resource, $"{scopeName}:{scopeId}:paged:{pageNumber}:{pageSize}");

    internal CacheKey ById(int id) => CacheKey.Plain(_resource, $"id:{id}");
}
