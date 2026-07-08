using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Books;
using Library.Domain.Services.AuthorsService;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Services.BooksService;

public class BooksService : BaseService, IBooksService
{
    private readonly IBooksRepository _booksRepository;
    private readonly IAuthorsService _authorsService;

    public BooksService(IAuthorsService authorsService, IUnitOfWork unitOfWork, ILogger<BooksService> logger)
        : base(unitOfWork, logger)
    {
        _authorsService = authorsService ?? throw new ArgumentNullException(nameof(authorsService));
        _booksRepository = _unitOfWork.Books;
    }

    public async Task<BookDTO?> GetBookByIdAsync(int id)
    {
        _logger.LogInformation("Fetching book by ID: {BookId}", id);
        var book = await _booksRepository.GetByIdAsync(id);
        _logger.LogInformation("Book {BookId} {Found}", id, book != null ? "found" : "not found");
        return book;
    }

    public async Task<PagedResult<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(PagedRequest request)
    {
        _logger.LogInformation("Fetching books with authors. Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var totalItemsCount = await _booksRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            _logger.LogInformation("No items in requested page. Total books: {TotalCount}", totalItemsCount);
            return new PagedResult<BookWithAuthorsDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var books = await _booksRepository.GetBooksAsync(request);

        _logger.LogInformation("Successfully retrieved {ItemCount} books with authors out of {TotalCount}",
            books.Count, totalItemsCount);

        return new PagedResult<BookWithAuthorsDTO>
        {
            Items = books,
            TotalCount = totalItemsCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(PagedRequest request, int? publicationYear)
    {
        _logger.LogInformation("Fetching books with authors filtered by publication year: {Year}. Page: {PageNumber}, Size: {PageSize}",
            publicationYear, request.PageNumber, request.PageSize);

        var totalItemsCount = await _booksRepository.GetTotalEntriesByPublicationYear(publicationYear);

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            _logger.LogInformation("No items in requested page for year {Year}. Total: {TotalCount}",
                publicationYear, totalItemsCount);
            return new PagedResult<BookWithAuthorsDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var books = await _booksRepository.GetByPublicationYearAsync(request, publicationYear);

        _logger.LogInformation("Retrieved {ItemCount} books for year {Year}", books.Count, publicationYear);

        return new PagedResult<BookWithAuthorsDTO>
        {
            Items = books,
            TotalCount = totalItemsCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<BookReviewStatsDTO>> GetBookWithMinReviewCountAsync(PagedRequest request, int? minReviewCount)
    {
        _logger.LogInformation("Fetching books with minimum review count: {MinReviews}. Page: {PageNumber}, Size: {PageSize}",
            minReviewCount, request.PageNumber, request.PageSize);

        var totalItemsCount = await _booksRepository.GetTotalEntriesByMinReviewCount(minReviewCount);

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            _logger.LogInformation("No items in requested page for min reviews {MinReviews}. Total: {TotalCount}",
                minReviewCount, totalItemsCount);
            return new PagedResult<BookReviewStatsDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var books = await _booksRepository.GetByMinReviewCountAsync(request, minReviewCount);

        _logger.LogInformation("Retrieved {ItemCount} books with min {MinReviews} reviews",
            books.Count, minReviewCount);

        return new PagedResult<BookReviewStatsDTO>
        {
            Items = books,
            TotalCount = totalItemsCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

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
                author.Id = await _unitOfWork.Authors.AddAsync(author);
            }

            await _unitOfWork.Books.AddAsync(newBook);
        });

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