using Library.Validation;
using Library.ViewModels.Authors;

namespace Library.ViewModels.Books;

public class BookByYearItemViewModel : BaseBookViewModel
{
    public List<AuthorViewModel> Authors { get; set; } = [];
}

public class BooksByYearViewModel
{
    [PublicationYear]
    public int? PublicationYear { get; set; }

    public List<BookByYearItemViewModel> Results { get; set; } = [];
}
