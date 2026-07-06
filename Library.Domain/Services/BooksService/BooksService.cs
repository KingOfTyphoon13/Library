using Library.Domain.DataAccess;
using Library.Domain.DTOs.Books;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.BooksService;

public class BooksService : BaseService, IBooksService
{
    public BooksService(IUnitOfWork unitOfWork, ILogger<BaseService> logger) : base(unitOfWork, logger)
    {
    }

    public Task<BookDTO?> GetBookByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(int? publicationYear)
    {
        throw new NotImplementedException();
    }

    public Task<List<BookWithAuthorsDTO>> GetBookWithAuthorsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<BookWithAuthorsDTO>> GetBookWithMinReviewCountAsync(int? minReviewCount)
    {
        throw new NotImplementedException();
    }
    public Task AddBook(CreateBookDTO newBook)
    {
        throw new NotImplementedException();
    }
}
