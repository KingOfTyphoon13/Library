using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Reviews;

namespace Library.DAL.QueryBuilder.Directors;

internal static class ReviewQueryDirector
{
    public const string Table = "reviews";

    public static class Columns
    {
        public const string Id = "id";
        public const string BookId = "book_id";
        public const string Score = "score";
        public const string Title = "title";
        public const string PublicationYear = "publication_year";
        public const string AuthorId = "author_id";
        public const string Name = "name";
        public const string Surname = "surname";
    }

    public static BuiltQuery GetPagedReviewsWithBookInfo(PagedRequest request)
    {
        var subquery = QueryBuilder.Create()
            .Select(Columns.Id, Columns.BookId, Columns.Score)
            .From(Table)
            .OrderBy(Columns.Id)
            .Paginate(request)
            .Build();

        return QueryBuilder.Create()
            .Select(
                $"r.id AS {Columns.Id}", $"r.book_id AS {Columns.BookId}", $"r.score AS {Columns.Score}",
                $"b.title AS {Columns.Title}", $"b.publication_year AS {Columns.PublicationYear}",
                $"a.id AS {Columns.AuthorId}", $"a.name AS {Columns.Name}", $"a.surname AS {Columns.Surname}")
            .From(subquery, "r")
            .InnerJoin("books b", "b.id = r.book_id")
            .InnerJoin("bookauthors ba", "ba.book_id = b.id")
            .InnerJoin("authors a", "a.id = ba.author_id")
            .OrderBy("r.id")
            .Build();
    }

    public static BuiltQuery Insert(ReviewDTO dto)
    {
        return InsertQueryBuilder.Create()
            .Into(Table)
            .Value(Columns.BookId, "@BookId", dto.BookId)
            .Value(Columns.Score, "@Score", dto.Score)
            .OutputInserted("id")
            .Build();
    }

    public static BuiltQuery GetTotalCount()
    {
        return QueryBuilder.Create()
            .Select("COUNT(*)")
            .From(Table)
            .Build();
    }
}
