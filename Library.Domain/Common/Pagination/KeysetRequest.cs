namespace Library.Domain.Common.Pagination;

public class KeysetRequest
{
    public required int LastItemIndex { get; init; }

    public required int PageSize { get; init; }
}
