namespace Library.Domain.Services.CacheService.Keys;

internal static class CacheKeys
{
    private const string _AuthorsNamespace = "authors";
    private const string _BooksNamespace = "books";
    private const string _ReviewsNamespace = "reviews";

    internal static readonly CacheKeyBuilder Authors = new(_AuthorsNamespace);
    internal static readonly CacheKeyBuilder Books = new(_BooksNamespace);
    internal static readonly CacheKeyBuilder Reviews = new(_ReviewsNamespace);
}
