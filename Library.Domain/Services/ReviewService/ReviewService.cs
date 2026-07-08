using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Reviews;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.ReviewService;

public class ReviewService : BaseService, IReviewService
{
    private readonly IReviewsRepository _reviewRepository;

    public ReviewService(IUnitOfWork unitOfWork, ILogger<ReviewService> logger)
        : base(unitOfWork, logger)
    {
        _reviewRepository = _unitOfWork.Reviews;
    }

    public async Task<PagedResult<ReviewWithBookInfoDTO>> GetReviewsAsync(PagedRequest request)
    {
        _logger.LogInformation("Fetching reviews - Page: {PageNumber}, Size: {PageSize}",
            request.PageNumber, request.PageSize);

        var totalItemsCount = await _reviewRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            _logger.LogInformation("No reviews to return for page {PageNumber} (total items: {TotalCount})",
                request.PageNumber, totalItemsCount);

            return new PagedResult<ReviewWithBookInfoDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var reviews = await _reviewRepository.GetReviewsAsync(request);

        _logger.LogInformation("Successfully retrieved {ReviewCount} reviews out of {TotalCount} total",
            reviews.Count(), totalItemsCount);

        return new PagedResult<ReviewWithBookInfoDTO>
        {
            Items = reviews,
            TotalCount = totalItemsCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<int> SaveReviewAsync(ReviewDTO review)
    {
        if (review == null)
        {
            _logger.LogWarning("Attempted to save null review");
            throw new ArgumentNullException(nameof(review));
        }

        _logger.LogInformation("Saving review for BookId: {BookId}, Score: {Score}",
            review.BookId, review.Score);

        try
        {
            var id = await _reviewRepository.AddAsync(review);

            _logger.LogInformation("Review saved successfully with Id: {ReviewId}", id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save review for BookId: {BookId}", review.BookId);
            throw;
        }
    }
}
