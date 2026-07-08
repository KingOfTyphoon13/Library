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

    public BooksService(IAuthorsService authorsService, IUnitOfWork unitOfWork, ILogger<BaseService> logger) : base(unitOfWork, logger)
    {
        _authorsService = authorsService ?? throw new ArgumentNullException(nameof(authorsService));

        _booksRepository = _unitOfWork.Books;
    }

    public async Task<BookDTO?> GetBookByIdAsync(int id)
    {
        return await _booksRepository.GetByIdAsync(id);
    }

    public async Task<PagedResult<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(PagedRequest request)
    {
        var totalItemsCount = await _booksRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            return new PagedResult<BookWithAuthorsDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var books = await _booksRepository.GetBooksAsync(request);

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
        var totalItemsCount = await _booksRepository.GetTotalEntriesByPublicationYear(publicationYear);

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            return new PagedResult<BookWithAuthorsDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var books = await _booksRepository.GetByPublicationYearAsync(request, publicationYear);

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
        var totalItemsCount = await _booksRepository.GetTotalEntriesByMinReviewCount(minReviewCount);

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            return new PagedResult<BookReviewStatsDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var books = await _booksRepository.GetByMinReviewCountAsync(request, minReviewCount);

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
        if (newBook.Authors.Count > 5)
            throw new ValidationException("Max 5 authors per book.");

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            foreach (var author in newBook.Authors.Where(a => a.Id == 0))
                author.Id = await _unitOfWork.Authors.AddAsync(author);

            await _unitOfWork.Books.AddAsync(newBook);
        });

    }

    public async Task<int> GetBooksNumberAsync() => await _booksRepository.GetTotalEntriesAsync();
}
