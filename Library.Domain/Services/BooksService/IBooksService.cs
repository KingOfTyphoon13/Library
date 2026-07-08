using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Books;

namespace Library.Domain.Services.BooksService;

public interface IBooksService
{

    Task<PagedResult<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(PagedRequest request);

    Task<PagedResult<BookWithAuthorsDTO>> GetBooksWithAuthorsAsync(PagedRequest request, int? publicationYear);

    Task<PagedResult<BookReviewStatsDTO>> GetBookWithMinReviewCountAsync(PagedRequest request, int? minReviewCount);

    Task<int> GetBooksNumberAsync();

    Task<BookDTO?> GetBookByIdAsync(int id);

    Task AddBookAsync(CreateBookDTO newBook);
}
