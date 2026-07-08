namespace Library.ViewModels.Common.Pagination;

public class PaginatedViewModel<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public PaginationViewModel Pagination { get; set; } = new();
}
