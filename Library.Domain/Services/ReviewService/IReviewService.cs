using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Reviews;

namespace Library.Domain.Services.ReviewService;

public interface IReviewService
{
    Task<PagedResult<ReviewWithBookInfoDTO>> GetReviewsAsync(PagedRequest request);

    Task SaveReviewAsync(ReviewDTO review);
}
