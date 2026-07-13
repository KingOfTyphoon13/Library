using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.Services.CacheService;
using Library.Domain.Services.CacheService.Keys;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services;

public abstract class BaseService
{
    protected readonly IUnitOfWork _unitOfWork;

    protected readonly ICacheService _cacheService;

    protected readonly ILogger<BaseService> _logger;

    protected BaseService(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<BaseService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected async Task<PagedResult<TItem>> GetOrSetPagedAsync<TItem>(
        CacheKey cacheKey,
        PagedRequest request,
        string logContext,
        Func<Task<int>> getTotalCount,
        Func<Task<List<TItem>>> getItems)
    {
        _logger.LogInformation("Fetching {Context}. Page: {PageNumber}, Size: {PageSize}",
            logContext, request.PageNumber, request.PageSize);

        var cached = await _cacheService.GetAsync<PagedResult<TItem>>(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache hit for {Context}. Page: {PageNumber}", logContext, request.PageNumber);
            return cached;
        }

        var totalItemsCount = await getTotalCount();
        var isOutOfRange = totalItemsCount <= (request.PageNumber - 1) * request.PageSize;

        var items = isOutOfRange ? [] : await getItems();

        _logger.LogInformation("Retrieved {ItemCount} items for {Context} out of {TotalCount}",
            items.Count, logContext, totalItemsCount);

        var result = new PagedResult<TItem>
        {
            Items = items,
            TotalCount = totalItemsCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        await _cacheService.SetAsync(cacheKey, result);
        return result;
    }
}
