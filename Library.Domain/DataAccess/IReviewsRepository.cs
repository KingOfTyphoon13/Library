using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Reviews;

namespace Library.Domain.DataAccess;

public interface IReviewsRepository : IRepository
{
    Task<List<ReviewWithBookInfoDTO>> GetReviewsAsync(PagedRequest request);

    Task<int> AddAsync(ReviewDTO dto);
}
