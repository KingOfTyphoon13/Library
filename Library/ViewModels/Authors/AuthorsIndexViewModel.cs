using Library.ViewModels.Common;

namespace Library.ViewModels.Authors;

public class AuthorsIndexViewModel
{
    public List<AuthorListItemViewModel> Authors { get; set; } = [];
    public PaginationViewModel Pagination { get; set; } = new();
}
