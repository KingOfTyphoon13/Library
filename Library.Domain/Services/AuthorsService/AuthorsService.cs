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

    public Task<PagedResult<AuthorDTO>> GetAuthorsAsync(PagedRequest request) =>
    GetOrSetPagedAsync(
        CacheKeys.Authors.Paged(request),
        request,
        "authors",
        _authorRepository.GetTotalEntriesAsync,
        () => _authorRepository.GetAuthors(request));

    public Task<PagedResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(PagedRequest request) =>
        GetOrSetPagedAsync(
            CacheKeys.Authors.Paged(request, "WithBookCount"),
            request,
            "authors with book counts",
            _authorRepository.GetTotalEntriesAsync,
            () => _authorRepository.GetAllWithBookCountAsync(request));

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