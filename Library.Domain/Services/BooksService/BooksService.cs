using Library.Domain.DataAccess;
using Library.Domain.DTOs.Books;
using Library.Domain.Services.AuthorsService;
using Microsoft.Extensions.Logging;

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

    public async Task<List<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync()
    {
        return await _booksRepository.GetBooksAsync();
    }

    public async Task<List<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(int? publicationYear)
    {
        var books = await _booksRepository.GetByPublicationYearAsync((int)publicationYear);
        return books;
    }

    public async Task<List<BookReviewStatsDTO>> GetBookWithMinReviewCountAsync(int? minReviewCount)
    {
        var books = await _booksRepository.GetByMinReviewCountAsync((int)minReviewCount);
        return books;
    }
    public async Task AddBook(CreateBookDTO newBook)
    {
        await _unitOfWork.BeginTransactionAsync();

        foreach (var author in newBook.Authors)
        {
            if (author.Id == 0)
                await _authorsService.AddAuthorAsync(author);
        }

        await _booksRepository.AddAsync(newBook);

        await _unitOfWork.CommitAsync();
    }
}
