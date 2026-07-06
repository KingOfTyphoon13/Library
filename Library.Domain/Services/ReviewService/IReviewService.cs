using Library.Domain.DTOs.Reviews;

namespace Library.Domain.Services.ReviewService;

public interface IReviewService
{
    Task<List<ReviewDTO>> GetReviewsAsync();

    Task SaveReviewAsync(ReviewDTO review);
}
