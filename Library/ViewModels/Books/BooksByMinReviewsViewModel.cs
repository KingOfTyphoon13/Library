using Library.ViewModels.Common.Pagination;

namespace Library.ViewModels.Books;

public class BookReviewStatsItemViewModel : BookBaseViewModel
{
    public int ReviewCount { get; set; }
    public double AverageScore { get; set; }
}

public class BooksByMinReviewsViewModel
{
    public int? MinReviewCount { get; set; }
    public PaginatedViewModel<BookReviewStatsItemViewModel> Paginated { get; set; }
}
