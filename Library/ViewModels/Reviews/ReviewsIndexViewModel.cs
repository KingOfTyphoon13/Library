using Library.ViewModels.Common.Pagination;

namespace Library.ViewModels.Reviews;

public class ReviewsIndexViewModel
{
    public PaginatedViewModel<ReviewListItemViewModel> RecentReviews { get; set; }
}
