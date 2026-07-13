namespace Library.Domain.Services.CacheService.Keys;

internal sealed class CacheKeyBuilder
{
    internal readonly string Resource;

    internal CacheKeyBuilder(string resource) => Resource = resource;

    internal CacheKey Paged(int pageNumber, int pageSize) =>
        CacheKey.Versioned(Resource, $"paged:{pageNumber}:{pageSize}");

    internal CacheKey Keyset(int? lastItemId, int pageSize) =>
        CacheKey.Versioned(Resource, $"keyset:{lastItemId?.ToString() ?? "start"}:{pageSize}");

    internal CacheKey PagedScoped(string scopeName, int scopeId, int pageNumber, int pageSize) =>
        CacheKey.Versioned(Resource, $"{scopeName}:{scopeId}:paged:{pageNumber}:{pageSize}");

    internal CacheKey ById(int id) => CacheKey.Plain(Resource, $"id:{id}");
}
