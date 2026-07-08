using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Reviews;
using Microsoft.Extensions.Logging;

namespace Library.Domain.Services.ReviewService;

public class ReviewService : BaseService, IReviewService
{
    private readonly IReviewsRepository _reviewRepository;

    public ReviewService(IUnitOfWork unitOfWork, ILogger<BaseService> logger) : base(unitOfWork, logger)
    {
        _reviewRepository = _unitOfWork.Reviews;
    }

    public async Task<PagedResult<ReviewWithBookInfoDTO>> GetReviewsAsync(PagedRequest request)
    {
        var totalItemsCount = await _reviewRepository.GetTotalEntriesAsync();

        if (totalItemsCount <= (request.PageNumber - 1) * request.PageSize)
        {
            return new PagedResult<ReviewWithBookInfoDTO>
            {
                Items = [],
                TotalCount = totalItemsCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        var reviews = await _reviewRepository.GetReviewsAsync(request);

        return new PagedResult<ReviewWithBookInfoDTO>
        {
            Items = reviews,
            TotalCount = totalItemsCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task SaveReviewAsync(ReviewDTO review)
    {
        var id = await _reviewRepository.AddAsync(review);
    }
}
