using Library.ViewModels.Books;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.ViewModels.Reviews;

public class BookOptionViewModel : BaseBookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class CreateReviewViewModel : BaseReviewViewModel
{
    [BindNever]
    public List<BookOptionViewModel> AvailableBooks { get; set; } = [];
}
