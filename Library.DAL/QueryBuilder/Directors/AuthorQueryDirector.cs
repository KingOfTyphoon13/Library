using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Authors;

namespace Library.DAL.QueryBuilder.Directors;

public static class AuthorQueryDirector
{
    public const string Table = "authors";

    public static class Columns
    {
        public const string Id = "id";
        public const string Name = "name";
        public const string Surname = "surname";
        public const string BooksCount = "books_count";
    }

    public static BuiltQuery GetPagedWithBookCount(PagedRequest request)
    {
        return QueryBuilder.Create()
            .Select($"a.id AS {Columns.Id}", $"a.name AS {Columns.Name}",
                    $"a.surname AS {Columns.Surname}", $"COUNT(b.id) AS {Columns.BooksCount}")
            .From($"{Table} a")
            .LeftJoin("bookauthors ba", "ba.author_id = a.id")
            .LeftJoin("books b", "b.id = ba.book_id")
            .GroupBy("a.id", "a.name", "a.surname")
            .OrderBy("a.id")
            .Paginate(request)
            .Build();
    }

    public static BuiltQuery GetKeysetWithBookCount(KeysetRequest request)
    {
        return QueryBuilder.Create()
            .Select($"a.id AS {Columns.Id}", $"a.name AS {Columns.Name}",
                    $"a.surname AS {Columns.Surname}", $"COUNT(b.id) AS {Columns.BooksCount}")
            .From($"{Table} a")
            .LeftJoin("bookauthors ba", "ba.author_id = a.id")
            .LeftJoin("books b", "b.id = ba.book_id")
            .PaginateKeyset(request, "a.id")
            .GroupBy("a.id", "a.name", "a.surname")
            .OrderBy("a.id")
            .Build();
    }

    public static BuiltQuery GetPaged(PagedRequest request)
    {
        return QueryBuilder.Create()
            .Select($"a.id AS {Columns.Id}", $"a.name AS {Columns.Name}", $"a.surname AS {Columns.Surname}")
            .From($"{Table} a")
            .OrderBy("a.id")
            .Paginate(request)
            .Build();
    }

    public static BuiltQuery GetKeyset(KeysetRequest request)
    {
        return QueryBuilder.Create()
            .Select($"a.id AS {Columns.Id}", $"a.name AS {Columns.Name}", $"a.surname AS {Columns.Surname}")
            .From($"{Table} a")
            .PaginateKeyset(request, "a.id")
            .OrderBy("a.id")
            .Build();
    }

    public static BuiltQuery GetTotalCount()
    {
        return QueryBuilder.Create()
            .Select("COUNT(a.id)")
            .From($"{Table} a")
            .Build();
    }

    public static BuiltQuery ExistsById(int id)
    {
        return QueryBuilder.Create()
            .Select("1")
            .From(Table)
            .Where("id = @Id", "@Id", id)
            .Build();
    }

    public static BuiltQuery Insert(AuthorDTO dto)
    {
        return InsertQueryBuilder.Create()
            .Into(Table)
            .Value(Columns.Name, "@Name", dto.Name)
            .Value(Columns.Surname, "@Surname", dto.Surname)
            .OutputInserted("id")
            .Build();
    }
}