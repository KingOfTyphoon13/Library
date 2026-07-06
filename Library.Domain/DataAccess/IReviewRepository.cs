using Library.Domain.DTOs.Reviews;

namespace Library.Domain.DataAccess;

public interface IReviewRepository
{
    Task<int> AddAsync(ReviewDTO dto);

    Task<List<ReviewDTO>> GetReviewsAsync();
}
