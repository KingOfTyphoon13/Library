using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.AuthorsService;

public class AuthorsService : BaseService, IAuthorsService
{
    private readonly IAuthorsRepository _authorRepository;

    public AuthorsService(IUnitOfWork unitOfWork, ILogger<AuthorsService> logger)
        : base(unitOfWork, logger)
    {
        _authorRepository = _unitOfWork.Authors;
    }

    public async Task<PagedResult<AuthorDTO>> GetAuthorsAsync(PagedRequest request)
    {
        _logger.LogInformation("Fetching authors with paging. Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var totalItemsCount = await _authorRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            _logger.LogInformation("No items in requested page. Total count: {TotalCount}", totalItemsCount);
            return new PagedResult<AuthorDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };
        }

        var result = await _authorRepository.GetAuthors(request);

        _logger.LogInformation("Successfully retrieved {ItemCount} authors out of {TotalCount}",
            result.Count, totalItemsCount);

        return new PagedResult<AuthorDTO>
        {
            Items = result,
            TotalCount = totalItemsCount,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber
        };
    }

    public async Task<PagedResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(PagedRequest request)
    {
        _logger.LogInformation("Fetching authors with book counts. Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var totalItemsCount = await _authorRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            _logger.LogInformation("No items in requested page for authors with book counts. Total: {TotalCount}", totalItemsCount);
            return new PagedResult<AuthorWithBooksCountDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };
        }

        var result = await _authorRepository.GetAllWithBookCountAsync(request);

        _logger.LogInformation("Successfully retrieved {ItemCount} authors with book counts", result.Count);

        return new()
        {
            Items = result,
            TotalCount = totalItemsCount,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber
        };
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

        _logger.LogInformation("Successfully added new author");

        return await _authorRepository.AddAsync(newAuthor);
    }
}