using Library.Domain.DTOs.Reviews;

namespace Library.Domain.Services.ReviewService;

public interface IReviewService
{
    List<ReviewDTO> GetReviews();

    void SaveReview(ReviewDTO review);
}
