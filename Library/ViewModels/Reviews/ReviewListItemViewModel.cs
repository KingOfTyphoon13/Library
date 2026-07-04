using Library.ViewModels.Books;

namespace Library.ViewModels.Reviews;

public class ReviewListItemViewModel : ReviewBaseViewModel
{
    public BookSummaryViewModel BookSummary { get; set; } = new();
}