using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Books;

namespace Library.Domain.DataAccess;

public interface IBooksRepository : IRepository
{
    Task<List<BookWithAuthorsDTO>> GetBooksAsync(PagedRequest request);

    Task<List<BookWithAuthorsDTO>> GetByPublicationYearAsync(PagedRequest request, int? year);

    Task<List<BookReviewStatsDTO>> GetByMinReviewCountAsync(PagedRequest request, int? minReviews);

    Task<BookDTO?> GetByIdAsync(int id);

    Task<int> GetTotalEntriesByPublicationYear(int? year);

    Task<int> GetTotalEntriesByMinReviewCount(int? minReviews);

    Task<int> AddAsync(CreateBookDTO dto);
}
