using Library.Domain.DTOs.Reviews;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.ReviewService;

public class ReviewService : BaseService, IReviewService
{

    public ReviewService(ILogger<BaseService> logger) : base(logger)
    {
    }

    public List<ReviewDTO> GetReviews()
    {
        throw new NotImplementedException();
    }

    public void SaveReview(ReviewDTO review)
    {
        throw new NotImplementedException();
    }
}
