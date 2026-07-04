using Library.Validation;

namespace Library.ViewModels.Books;

public class BooksByYearViewModel
{
    [PublicationYear]
    public int? PublicationYear { get; set; }

    public List<BookSummaryViewModel> Results { get; set; } = [];
}
