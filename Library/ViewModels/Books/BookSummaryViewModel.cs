using Library.ViewModels.Authors;

namespace Library.ViewModels.Books;

public class BookSummaryViewModel : BookBaseViewModel
{
    public List<AuthorViewModel> Authors { get; set; } = [];
}
