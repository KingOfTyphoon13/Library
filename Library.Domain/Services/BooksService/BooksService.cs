using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Books;
using Library.Domain.Services.AuthorsService;
using Library.Domain.Services.CacheService;
using Library.Domain.Services.CacheService.Keys;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Services.BooksService;

public class BooksService : BaseService, IBooksService
{
    private readonly IBooksRepository _booksRepository;
    private readonly IAuthorsService _authorsService;

    public BooksService(IAuthorsService authorsService, IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<BooksService> logger)
        : base(unitOfWork, cacheService, logger)
    {
        _authorsService = authorsService ?? throw new ArgumentNullException(nameof(authorsService));
        _booksRepository = _unitOfWork.Books;
    }

    public async Task<BookDTO?> GetBookByIdAsync(int id)
    {
        _logger.LogInformation("Fetching book by ID: {BookId}", id);

        var cacheKey = CacheKeys.Books.ById(id);
        var cached = await _cacheService.GetAsync<BookDTO>(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Cache hit for book {BookId}", id);
            return cached;
        }

        var book = await _booksRepository.GetByIdAsync(id);
        _logger.LogInformation("Book {BookId} {Found}", id, book != null ? "found" : "not found");

        if (book is not null)
            await _cacheService.SetAsync(cacheKey, book);

        return book;
    }

    public Task<PagedResult<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(PagedRequest request) =>
        GetOrSetPagedAsync(
            CacheKeys.Books.Paged(request),
            request,
            "books with authors",
            _booksRepository.GetTotalEntriesAsync,
            () => _booksRepository.GetBooksAsync(request));

    public Task<PagedResult<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(PagedRequest request, int? publicationYear) =>
        GetOrSetPagedAsync(
            CacheKeys.Books.PagedScoped("year", publicationYear?.ToString() ?? "any", request),
            request,
            $"books filtered by year {publicationYear}",
            () => _booksRepository.GetTotalEntriesByPublicationYear(publicationYear),
            () => _booksRepository.GetByPublicationYearAsync(request, publicationYear));

    public Task<PagedResult<BookReviewStatsDTO>> GetBookWithMinReviewCountAsync(PagedRequest request, int? minReviewCount) =>
        GetOrSetPagedAsync(
            CacheKeys.Books.PagedScoped("minReviews", minReviewCount?.ToString() ?? "any", request),
            request,
            $"books with min reviews {minReviewCount}",
            () => _booksRepository.GetTotalEntriesByMinReviewCount(minReviewCount),
            () => _booksRepository.GetByMinReviewCountAsync(request, minReviewCount));

    public async Task AddBookAsync(CreateBookDTO newBook)
    {
        _logger.LogInformation("Adding new book: {Title} with {AuthorCount} authors",
            newBook.Title, newBook.Authors?.Count ?? 0);

        if (newBook.Authors.Count > 5)
        {
            _logger.LogWarning("Validation failed: Book has too many authors ({Count}). Max allowed: 5",
                newBook.Authors.Count);
            throw new ValidationException("Max 5 authors per book.");
        }

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            foreach (var author in newBook.Authors.Where(a => a.Id == 0))
            {
                _logger.LogDebug("Adding new author for book: {AuthorName}", author.Name);
                author.Id = await _authorsService.AddAuthorAsync(author); // already bumps authors:version internally
            }

            await _unitOfWork.Books.AddAsync(newBook);
        });

        await _cacheService.InvalidateAsync(CacheKeys.Books.Resource);

        _logger.LogInformation("Successfully added book: {Title}", newBook.Title);
    }

    public async Task<int> GetBooksNumberAsync()
    {
        _logger.LogDebug("Retrieving total number of books");
        var count = await _booksRepository.GetTotalEntriesAsync();
        _logger.LogInformation("Total books count: {TotalCount}", count);
        return count;
    }
}