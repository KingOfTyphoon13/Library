using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.AuthorsService;

public class AuthorsService : BaseService, IAuthorsService
{
    private readonly IAuthorsRepository _authorRepository;

    public AuthorsService(IUnitOfWork unitOfWork, ILogger<AuthorsService> logger) : base(unitOfWork, logger)
    {
        _authorRepository = _unitOfWork.Authors;
    }

    public async Task<PagedResult<AuthorDTO>> GetAuthorsAsync(PagedRequest request)
    {
        var totalItemsCount = await _authorRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            return new PagedResult<AuthorDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };
        }

        var result = await _authorRepository.GetAuthors(request);

        return new PagedResult<AuthorDTO>
        {
            Items = result,
            TotalCount = totalItemsCount,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber
        };
    }

    public async Task<KeysetResult<AuthorDTO>> GetAuthorsAsync(KeysetRequest request)
    {
        var fetchRequest = new KeysetRequest
        {
            LastItemIndex = request.LastItemIndex,
            PageSize = request.PageSize + 1
        };

        var result = await _authorRepository.GetAuthors(fetchRequest);

        var hasNextPage = result.Count > request.PageSize;
        if (hasNextPage)
        {
            result.RemoveAt(result.Count - 1);
        }

        return new KeysetResult<AuthorDTO>
        {
            Items = result,
            PageSize = request.PageSize,
            HasNextPage = hasNextPage
        };
    }

    public async Task<PagedResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(PagedRequest request)
    {
        var totalItemsCount = await _authorRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            return new PagedResult<AuthorWithBooksCountDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageSize = request.PageSize,
                PageNumber = request.PageNumber
            };
        }

        var result = await _authorRepository.GetAllWithBookCountAsync(request);

        return new()
        {
            Items = result,
            TotalCount = totalItemsCount,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber
        };
    }

    public async Task<KeysetResult<AuthorWithBooksCountDTO>> GetAuthorWithBooksCountsAsync(KeysetRequest request)
    {
        var fetchRequest = new KeysetRequest
        {
            LastItemIndex = request.LastItemIndex,
            PageSize = request.PageSize + 1
        };

        var result = await _authorRepository.GetAllWithBookCountAsync(fetchRequest);

        var hasNextPage = result.Count > request.PageSize;
        if (hasNextPage)
        {
            result.RemoveAt(result.Count - 1);
        }

        return new KeysetResult<AuthorWithBooksCountDTO>
        {
            Items = result,
            PageSize = request.PageSize,
            HasNextPage = hasNextPage
        };
    }

    public async Task<int> GetAuthorsNumberAsync() => await _authorRepository.GetTotalEntriesAsync();

    public async Task AddAuthorAsync(AuthorDTO newAuthor)
    {
        if (string.IsNullOrWhiteSpace(newAuthor.Name) && string.IsNullOrWhiteSpace(newAuthor.Surname))
            throw new ArgumentException("Either Name or Surname must be provided.", nameof(newAuthor));

        await _authorRepository.AddAsync(newAuthor);
    }
}
