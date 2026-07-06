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

    public async Task<List<ReviewWithBookInfoDTO>> GetReviewsAsync()
    {

        return await _reviewRepository.GetReviewsAsync();
    }

    public Task SaveReviewAsync(ReviewDTO review)
    {
        throw new NotImplementedException();
    }
}
