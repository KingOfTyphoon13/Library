using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Books;

namespace Library.DAL.QueryBuilder.Directors;

public static class BookQueryDirector
{
    public const string Table = "books";

    public static class Columns
    {
        public const string Id = "id";
        public const string BookId = "book_id";
        public const string Title = "title";
        public const string PublicationYear = "publication_year";
        public const string AuthorId = "author_id";
        public const string Name = "name";
        public const string Surname = "surname";
        public const string ReviewCount = "review_count";
        public const string AverageScore = "average_score";
    }

    public static BuiltQuery GetPagedWithAuthors(PagedRequest request)
    {
        var subquery = QueryBuilder.Create()
            .Select(Columns.Id, Columns.Title, Columns.PublicationYear)
            .From(Table)
            .OrderBy(Columns.Id)
            .Paginate(request)
            .Build();

        return QueryBuilder.Create()
            .Select(
                $"b.id AS {Columns.Id}", $"b.title AS {Columns.Title}", $"b.publication_year AS {Columns.PublicationYear}",
                $"a.id AS {Columns.AuthorId}", $"a.name AS {Columns.Name}", $"a.surname AS {Columns.Surname}")
            .From(subquery, "b")
            .InnerJoin("bookauthors ba", "ba.book_id = b.id")
            .InnerJoin("authors a", "a.id = ba.author_id")
            .OrderBy("b.id, a.id")
            .Build();
    }

    public static BuiltQuery GetPagedByPublicationYear(PagedRequest request, int? year)
    {
        var subquery = QueryBuilder.Create()
            .Select(Columns.Id, Columns.Title, Columns.PublicationYear)
            .From(Table)
            .Where("(@PublicationYear IS NULL OR publication_year = @PublicationYear)", "@PublicationYear", year)
            .OrderBy(Columns.Id)
            .Paginate(request)
            .Build();

        return QueryBuilder.Create()
            .Select(
                $"b.id AS {Columns.Id}", $"b.title AS {Columns.Title}", $"b.publication_year AS {Columns.PublicationYear}",
                $"a.id AS {Columns.AuthorId}", $"a.name AS {Columns.Name}", $"a.surname AS {Columns.Surname}")
            .From(subquery, "b")
            .InnerJoin("bookauthors ba", "ba.book_id = b.id")
            .InnerJoin("authors a", "a.id = ba.author_id")
            .OrderBy("b.id, a.id")
            .Build();
    }

    public static BuiltQuery GetPagedByMinReviewCount(PagedRequest request, int? minReviews)
    {
        return QueryBuilder.Create()
            .Select(
                $"b.id AS {Columns.Id}", $"b.title AS {Columns.Title}", $"b.publication_year AS {Columns.PublicationYear}",
                $"COUNT(r.id) AS {Columns.ReviewCount}",
                $"ROUND(AVG(CAST(r.score AS float)), 2) AS {Columns.AverageScore}")
            .From("books b")
            .InnerJoin("reviews r", "r.book_id = b.id")
            .GroupBy("b.id", "b.title", "b.publication_year")
            .Having("(@MinReviews IS NULL OR COUNT(r.id) >= @MinReviews)", "@MinReviews", minReviews)
            .OrderBy("b.id")
            .Paginate(request)
            .Build();
    }

    public static BuiltQuery GetTotalCount()
    {
        return QueryBuilder.Create().Select("COUNT(*)").From(Table).Build();
    }

    public static BuiltQuery GetTotalCountByPublicationYear(int? year)
    {
        return QueryBuilder.Create()
            .Select("COUNT(*)")
            .From(Table)
            .Where("(@PublicationYear IS NULL OR publication_year = @PublicationYear)", "@PublicationYear", year)
            .Build();
    }

    public static BuiltQuery GetTotalCountByMinReviewCount(int? minReviews)
    {
        var inner = QueryBuilder.Create()
            .Select("b.id")
            .From("books b")
            .InnerJoin("reviews r", "r.book_id = b.id")
            .GroupBy("b.id")
            .Having("(@MinReviews IS NULL OR COUNT(r.id) >= @MinReviews)", "@MinReviews", minReviews)
            .Build();

        return QueryBuilder.Create()
            .Select("COUNT(*)")
            .From(inner, "counted")
            .Build();
    }

    public static BuiltQuery GetById(int id)
    {
        return QueryBuilder.Create()
            .Select(Columns.Id, Columns.Title, Columns.PublicationYear)
            .From(Table)
            .Where($"{Columns.Id} = @Id", "@Id", id)
            .Build();
    }

    public static BuiltQuery Insert(CreateBookDTO dto)
    {
        return InsertQueryBuilder.Create()
            .Into(Table)
            .Value(Columns.Title, "@Title", dto.Title)
            .Value(Columns.PublicationYear, "@Year", dto.PublicationYear)
            .OutputInserted(Columns.Id)
            .Build();
    }

    public static BuiltQuery InsertAuthorLinks(int bookId, IEnumerable<int> authorIds)
    {
        var builder = InsertQueryBuilder.Create().Into("bookauthors");
        foreach (var authorId in authorIds)
        {
            builder.AddRow((Columns.BookId, bookId), (Columns.AuthorId, authorId));
        }
        return builder.Build();
    }
}
