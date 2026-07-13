using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Reviews;
using Library.Domain.Services.CacheService;
using Library.Domain.Services.CacheService.Keys;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.ReviewService;

public class ReviewService : BaseService, IReviewService
{
    private readonly IReviewsRepository _reviewRepository;

    public ReviewService(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<ReviewService> logger)
        : base(unitOfWork, cacheService, logger)
    {
        _reviewRepository = _unitOfWork.Reviews;
    }

    public Task<PagedResult<ReviewWithBookInfoDTO>> GetReviewsAsync(PagedRequest request) =>
        GetOrSetPagedAsync(
            CacheKeys.Reviews.Paged(request),
            request,
            "reviews",
            _reviewRepository.GetTotalEntriesAsync,
            () => _reviewRepository.GetReviewsAsync(request));

    public async Task<int> SaveReviewAsync(ReviewDTO review)
    {
        if (review == null)
        {
            _logger.LogWarning("Attempted to save null review");
            throw new ArgumentNullException(nameof(review));
        }

        _logger.LogInformation("Saving review for BookId: {BookId}, Score: {Score}",
            review.BookId, review.Score);

        var id = await _reviewRepository.AddAsync(review);
        await _cacheService.InvalidateAsync(CacheKeys.Reviews.Resource);

        _logger.LogInformation("Review saved successfully with Id: {ReviewId}", id);
        return id;
    }
}
