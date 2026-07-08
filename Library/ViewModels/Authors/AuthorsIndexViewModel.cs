using Library.ViewModels.Common.Pagination;

namespace Library.ViewModels.Authors;

public class AuthorsIndexViewModel
{
    public PaginatedViewModel<AuthorListItemViewModel> AuthorsListViewModel { get; set; }
}
