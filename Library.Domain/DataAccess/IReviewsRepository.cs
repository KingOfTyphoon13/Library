using Library.Domain.DTOs.Reviews;

namespace Library.Domain.DataAccess;

public interface IReviewsRepository : IRepository
{
    Task<List<ReviewWithBookInfoDTO>> GetReviewsAsync();

    Task<int> AddAsync(ReviewDTO dto);
}
