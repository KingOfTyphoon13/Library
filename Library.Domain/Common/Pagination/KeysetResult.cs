namespace Library.Domain.Common.Pagination;

public interface IIdentifiable
{
    int Id { get; }
}

public class KeysetResult<T> where T : IIdentifiable
{
    public required IReadOnlyList<T> Items { get; init; }

    public int TotalCount { get; init; }

    public int PageSize { get; init; }

    public bool HasNextPage { get; init; }

    public int? LastItemId => Items.Count > 0 ? Items[^1].Id : null;
}
