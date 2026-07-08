using Library.Domain.DTOs.Books;

namespace Library.Domain.DataAccess;

public interface IBooksRepository : IRepository
{
    Task<List<BookWithAuthorsDTO>> GetBooksAsync();

    Task<List<BookWithAuthorsDTO>> GetByPublicationYearAsync(int? year);

    Task<List<BookReviewStatsDTO>> GetByMinReviewCountAsync(int? minReviews);

    Task<BookDTO?> GetByIdAsync(int id);

    Task<int> AddAsync(CreateBookDTO dto);
}
