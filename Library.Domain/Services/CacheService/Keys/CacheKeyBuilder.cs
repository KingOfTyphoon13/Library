using Library.Domain.Common.Pagination;

namespace Library.Domain.Services.CacheService.Keys;

internal sealed class CacheKeyBuilder
{
    internal readonly string Resource;

    internal CacheKeyBuilder(string resource) => Resource = resource;

    internal CacheKey Paged(PagedRequest request) =>
        CacheKey.Versioned(Resource, $"paged:{request.PageNumber}:{request.PageSize}");

    internal CacheKey Paged(PagedRequest request, string? variant = null) =>
        CacheKey.Versioned(Resource, $"paged{variant}:{request.PageNumber}:{request.PageSize}");

    internal CacheKey Keyset(KeysetRequest request) =>
        CacheKey.Versioned(Resource, $"keyset:{request.LastItemIndex}:{request.PageSize}");

    internal CacheKey PagedScoped(string scopeName, string scopeValue, PagedRequest request) =>
    CacheKey.Versioned(Resource, $"{scopeName}:{scopeValue}:paged:{request.PageNumber}:{request.PageSize}");

    internal CacheKey ById(int id) => CacheKey.Plain(Resource, $"id:{id}");
}
