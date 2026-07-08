using Library.Validation;
using Library.ViewModels.Common.Pagination;

namespace Library.ViewModels.Books;

public class BooksByYearViewModel
{
    [PublicationYear]
    public int? PublicationYear { get; set; }
    public PaginatedViewModel<BookSummaryViewModel> Paginated { get; set; }
}
