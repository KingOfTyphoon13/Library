using Library.Domain.DTOs.Reviews;

namespace Library.Domain.DataAccess;

public interface IReviewsRepository
{
    Task<List<ReviewDTO>> GetReviewsAsync();

    Task<int> AddAsync(ReviewDTO dto);
}
