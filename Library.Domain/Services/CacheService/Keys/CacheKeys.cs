namespace Library.Domain.Services.CacheService.Keys;

internal static class CacheKeys
{
    private const string Separator = ":";

    private static string Build(params object[] parts) => string.Join(Separator, parts);

    internal static class Authors
    {
        private const string VersionKey = "authors:version";

        internal static string Version() => VersionKey;

        internal static string Paged(long version, int pageNumber, int pageSize) =>
            Build("authors", "v" + version, "paged", pageNumber, pageSize);

        internal static string Keyset(long version, int? lastItemId, int pageSize) =>
            Build("authors", "v" + version, "keyset", lastItemId?.ToString() ?? "start", pageSize);

        internal static string ById(int id) => Build("authors", id);
    }

    internal static class Books
    {
        private const string VersionKey = "books:version";

        internal static string Version() => VersionKey;

        internal static string Paged(long version, int pageNumber, int pageSize) =>
            Build("books", "v" + version, "paged", pageNumber, pageSize);

        internal static string ById(int id) => Build("books", id);
    }

    internal static class Reviews
    {
        private const string VersionKey = "reviews:version";

        internal static string Version() => VersionKey;

        internal static string PagedByBook(long version, int bookId, int pageNumber, int pageSize) =>
            Build("reviews", "v" + version, "book", bookId, "paged", pageNumber, pageSize);
    }
}
