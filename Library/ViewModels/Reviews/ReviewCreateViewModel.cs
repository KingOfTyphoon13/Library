using Library.ViewModels.Books;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.ViewModels.Reviews;

public class ReviewCreateViewModel : ReviewBaseViewModel
{
    public BookSummaryViewModel? Book { get; set; }

    [BindNever]
    public List<BookSummaryViewModel> AvailableBooks { get; set; } = [];
}
