namespace Library.Domain.Common.Pagination;

public class PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }

    public required int TotalCount { get; init; }

    public required int PageSize { get; init; }

    public required int PageNumber { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
