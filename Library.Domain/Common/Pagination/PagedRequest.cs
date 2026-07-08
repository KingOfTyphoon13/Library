namespace Library.Domain.Common.Pagination;

public class PagedRequest
{
    public required int PageNumber { get; init; }

    public required int PageSize { get; init; }
}
