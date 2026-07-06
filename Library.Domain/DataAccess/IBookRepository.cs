using Library.Domain.DTOs.Books;

namespace Library.Domain.DataAccess;

public interface IBookRepository
{
    Task<int> AddAsync(CreateBookDTO dto);
    Task<BookDTO?> GetByIdAsync(int id);
    Task<IReadOnlyList<BookDTO>> GetByPublicationYearAsync(int year);
    Task<IReadOnlyList<BookReviewStatsDTO>> GetByMinReviewCountAsync(int minReviews);
    Task<int> GetAuthorCountAsync(int bookId);
}
