using Library.DAL.QueryBuilder;
using Library.DAL.QueryBuilder.Directors;
using Library.Domain.Common.Pagination;
using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
using Library.Domain.DTOs.Books;
using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal class BooksRepository : BaseRepository, IBooksRepository
{
    public BooksRepository(SqlConnection connection, Func<SqlTransaction?> transaction, Func<Task?> openDbConnection) : base(connection, transaction, openDbConnection)
    {
    }

    public async Task<int> AddAsync(CreateBookDTO dto)
    {
        await _openDbConnectionAsync();

        var insertBookQuery = BookQueryDirector.Insert(dto);
        int bookId;
        await using (var cmd = insertBookQuery.ToCommand(_connection, _transaction()))
        {
            bookId = (int)await cmd.ExecuteScalarAsync();
        }

        if (dto.Authors.Count > 0)
        {
            var insertLinksQuery = BookQueryDirector.InsertAuthorLinks(bookId, dto.Authors.Select(a => a.Id));
            await using var cmd = insertLinksQuery.ToCommand(_connection, _transaction());
            await cmd.ExecuteNonQueryAsync();
        }

        return bookId;
    }

    public async Task<List<BookWithAuthorsDTO>> GetBooksAsync(PagedRequest request)
    {
        var query = BookQueryDirector.GetPagedWithAuthors(request);
        return await ReadBooksWithAuthorsAsync(query);
    }

    public async Task<List<BookReviewStatsDTO>> GetByMinReviewCountAsync(PagedRequest request, int? minReviews)
    {
        var query = BookQueryDirector.GetPagedByMinReviewCount(request, minReviews);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal(BookQueryDirector.Columns.Id);
        var titleOrd = reader.GetOrdinal(BookQueryDirector.Columns.Title);
        var yearOrd = reader.GetOrdinal(BookQueryDirector.Columns.PublicationYear);
        var reviewCountOrd = reader.GetOrdinal(BookQueryDirector.Columns.ReviewCount);
        var averageScoreOrd = reader.GetOrdinal(BookQueryDirector.Columns.AverageScore);

        var books = new List<BookReviewStatsDTO>();
        while (await reader.ReadAsync())
        {
            books.Add(new BookReviewStatsDTO
            {
                Id = reader.GetInt32(idOrd),
                Title = reader.GetString(titleOrd),
                PublicationYear = reader.GetInt32(yearOrd),
                ReviewCount = reader.GetInt32(reviewCountOrd),
                AverageScore = reader.GetDouble(averageScoreOrd)
            });
        }

        return books;
    }

    public async Task<List<BookWithAuthorsDTO>> GetByPublicationYearAsync(PagedRequest request, int? year)
    {
        var query = BookQueryDirector.GetPagedByPublicationYear(request, year);
        return await ReadBooksWithAuthorsAsync(query);
    }

    public async Task<BookDTO?> GetByIdAsync(int id)
    {
        await _openDbConnectionAsync();

        var command = BookQueryDirector.GetById(id).ToCommand(_connection, _transaction());

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new BookDTO
        {
            Id = reader.GetInt32(reader.GetOrdinal(BookQueryDirector.Columns.Id)),
            Title = reader.GetString(reader.GetOrdinal(BookQueryDirector.Columns.Title)),
            PublicationYear = reader.GetInt32(reader.GetOrdinal(BookQueryDirector.Columns.PublicationYear))
        };
    }

    public async Task<int> GetTotalEntriesAsync()
    {
        var query = BookQueryDirector.GetTotalCount();
        await _openDbConnectionAsync();
        await using var command = query.ToCommand(_connection, _transaction());
        return (int)await command.ExecuteScalarAsync();
    }

    public async Task<int> GetTotalEntriesByPublicationYear(int? year)
    {
        var query = BookQueryDirector.GetTotalCountByPublicationYear(year);
        await _openDbConnectionAsync();
        await using var command = query.ToCommand(_connection, _transaction());
        return (int)await command.ExecuteScalarAsync();
    }

    public async Task<int> GetTotalEntriesByMinReviewCount(int? minReviews)
    {
        var query = BookQueryDirector.GetTotalCountByMinReviewCount(minReviews);
        await _openDbConnectionAsync();
        await using var command = query.ToCommand(_connection, _transaction());
        return (int)await command.ExecuteScalarAsync();
    }

    private async Task<List<BookWithAuthorsDTO>> ReadBooksWithAuthorsAsync(BuiltQuery query)
    {
        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal(BookQueryDirector.Columns.Id);
        var titleOrd = reader.GetOrdinal(BookQueryDirector.Columns.Title);
        var yearOrd = reader.GetOrdinal(BookQueryDirector.Columns.PublicationYear);
        var authorIdOrd = reader.GetOrdinal(BookQueryDirector.Columns.AuthorId);
        var nameOrd = reader.GetOrdinal(BookQueryDirector.Columns.Name);
        var surnameOrd = reader.GetOrdinal(BookQueryDirector.Columns.Surname);

        var books = new Dictionary<int, BookWithAuthorsDTO>();

        while (await reader.ReadAsync())
        {
            var bookId = reader.GetInt32(idOrd);

            if (!books.TryGetValue(bookId, out var dto))
            {
                dto = new BookWithAuthorsDTO
                {
                    Id = bookId,
                    Title = reader.GetString(titleOrd),
                    PublicationYear = reader.GetInt32(yearOrd)
                };
                books[bookId] = dto;
            }

            dto.Authors.Add(new AuthorDTO
            {
                Id = reader.GetInt32(authorIdOrd),
                Name = reader.GetString(nameOrd),
                Surname = reader.GetString(surnameOrd)
            });
        }

        return books.Values.ToList();
    }
}
