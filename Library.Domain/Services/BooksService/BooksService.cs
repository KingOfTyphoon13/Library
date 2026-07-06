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
        if (newBook.Authors.Count > 5)
            throw new ValidationException("Max 5 authors per book.");

        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            foreach (var author in newBook.Authors.Where(a => a.Id == 0))
                author.Id = await _unitOfWork.Authors.AddAsync(author);

            await _unitOfWork.Books.AddAsync(newBook);
        });

    }
}
