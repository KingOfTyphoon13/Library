using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
using Library.Domain.Services.CacheService;
using Library.Domain.Services.CacheService.Keys;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.AuthorsService;

public class AuthorsService : BaseService, IAuthorsService
{
    private readonly IAuthorsRepository _authorRepository;

    public AuthorsService(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<AuthorsService> logger)
        : base(unitOfWork, cacheService, logger)
    {
        _authorRepository = _unitOfWork.Authors;
    }

    public async Task<PagedResult<AuthorDTO>> GetAuthorsAsync(PagedRequest request)
    {
        _logger.LogInformation("Fetching authors with paging. Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var cacheKey = CacheKeys.Authors.Paged(request.PageNumber, request.PageSize);
        var cached = await _cacheService.GetAsync<PagedResult<AuthorDTO>>(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache hit for authors. Page: {PageNumber}", request.PageNumber);
            return cached;
        }

        var totalItemsCount = await _authorRepository.GetTotalEntriesAsync();
        var isOutOfRange = totalItemsCount <= (request.PageNumber - 1) * request.PageSize;
        var logMessage = string.Format("No items in requested page. Total count: {0}", totalItemsCount);

        List<AuthorDTO> items = [];

        if (!isOutOfRange)
        {
            items = await _authorRepository.GetAuthors(request);
            logMessage = string.Format("Successfully retrieved {0} authors out of {1}", items.Count, totalItemsCount);
        }

        _logger.LogInformation(logMessage);

        var result = new PagedResult<AuthorDTO>
        {
            Items = items,
            TotalCount = totalItemsCount,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber
        };

        await _cacheService.SetAsync(cacheKey, result);
        return result;
    }

    public async Task<PagedResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(PagedRequest request)
    {
        _logger.LogInformation("Fetching authors with book counts. Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var cacheKey = CacheKeys.Authors.Paged(request.PageNumber, request.PageSize);
        var cached = await _cacheService.GetAsync<PagedResult<AuthorWithBooksCountDTO>>(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache hit for authors with book counts. Page: {PageNumber}", request.PageNumber);
            return cached;
        }

        var totalItemsCount = await _authorRepository.GetTotalEntriesAsync();

        var isOutOfRange = totalItemsCount <= (request.PageNumber - 1) * request.PageSize;
        var items = isOutOfRange
            ? []
            : await _authorRepository.GetAllWithBookCountAsync(request);

        _logger.LogInformation(isOutOfRange
            ? "No items in requested page for authors with book counts. Total: {TotalCount}"
            : "Successfully retrieved {ItemCount} authors with book counts",
            isOutOfRange ? totalItemsCount : items.Count);

        var result = new PagedResult<AuthorWithBooksCountDTO>
        {
            Items = items,
            TotalCount = totalItemsCount,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber
        };

        await _cacheService.SetAsync(cacheKey, result);
        return result;
    }

    public async Task<int> GetAuthorsNumberAsync()
    {
        _logger.LogDebug("Retrieving total number of authors");
        var count = await _authorRepository.GetTotalEntriesAsync();
        _logger.LogInformation("Total authors count: {TotalCount}", count);
        return count;
    }

    public async Task<int> AddAuthorAsync(AuthorDTO newAuthor)
    {
        _logger.LogInformation("Adding new author. Name: {Name}, Surname: {Surname}",
            newAuthor.Name, newAuthor.Surname);

        if (string.IsNullOrWhiteSpace(newAuthor.Name) && string.IsNullOrWhiteSpace(newAuthor.Surname))
        {
            _logger.LogWarning("Invalid author data: Either Name or Surname must be provided.");
            throw new ArgumentException("Either Name or Surname must be provided.", nameof(newAuthor));
        }

        var id = await _authorRepository.AddAsync(newAuthor);

        await _cacheService.InvalidateAsync(CacheKeys.Authors.Resource);

        _logger.LogInformation("Successfully added new author");

        return id;
    }
}