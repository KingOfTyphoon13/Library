using Library.Domain.DTOs.Books;

namespace Library.Domain.Services.BooksService;

public interface IBooksService
{
    Task<List<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync();
    Task<List<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(int? publicationYear);
    Task<List<BookReviewStatsDTO>> GetBookWithMinReviewCountAsync(int? minReviewCount);

    Task<BookDTO?> GetBookByIdAsync(int id);

    Task AddBook(CreateBookDTO newBook);
}
