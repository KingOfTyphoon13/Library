using Library.Domain.DTOs.Reviews;

namespace Library.Domain.Services.ReviewService;

public interface IReviewService
{
    Task<List<ReviewWithBookInfoDTO>> GetReviewsAsync();

    Task SaveReviewAsync(ReviewDTO review);
}
