using Library.Domain.DTOs.Books;

namespace Library.Domain.Services.BooksService;

public interface IBooksService
{
    Task<List<BookWithAuthorsDTO>> GetBookWithAuthorsAsync();
    Task<List<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(int? publicationYear);
    Task<List<BookWithAuthorsDTO>> GetBookWithMinReviewCountAsync(int? minReviewCount);

    Task<BookDTO?> GetBookByIdAsync(int id);

    Task AddBook(CreateBookDTO newBook);
}
