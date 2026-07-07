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

    public Task<int> AddAsync(CreateBookDTO dto)
    {
        throw new NotImplementedException();
    }

    public async Task<List<BookWithAuthorsDTO>> GetBooksAsync()
    {
        const string query = "SELECT " +
            "                   b.id, b.title, b.publication_year," +
            "                   a.id AS \"author_id\", a.name, a.surname" +
            "                 From books b" +
            "                 Join bookauthors ba ON ba.book_id = b.id" +
            "                 Join authors a ON a.id = ba.author_id " +
            "                 Order by b.id";


        var books = new Dictionary<int, BookWithAuthorsDTO>();

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(query, _connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal("id");
        var titleOrd = reader.GetOrdinal("title");
        var yearOrd = reader.GetOrdinal("publication_year");
        var authorIdOrd = reader.GetOrdinal("author_id");
        var nameOrd = reader.GetOrdinal("name");
        var surnameOrd = reader.GetOrdinal("surname");

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

    public async Task<BookDTO?> GetByIdAsync(int id)
    {
        const string query = "SELECT " +
            "                   b.id, b.title, b.publication_year " +
            "                 From books b" +
            "                 Where b.id = @ID";


        await _openDbConnectionAsync();

        await using var command = new SqlCommand(query, _connection, _transaction());
        command.Parameters.Add(new SqlParameter("@ID", id));

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return null;

        return new BookDTO
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Title = reader.GetString(reader.GetOrdinal("title")),
            PublicationYear = reader.GetInt32(reader.GetOrdinal("publication_year"))
        };
    }

    public Task<List<BookReviewStatsDTO>> GetByMinReviewCountAsync(int? minReviews)
    {
        throw new NotImplementedException();
    }

    public async Task<List<BookWithAuthorsDTO>> GetByPublicationYearAsync(int year)
    {
        const string query = "SELECT " +
            "                   b.id, b.title, b.publication_year," +
            "                   a.id AS \"author_id\", a.name, a.surname" +
            "                 From books b" +
            "                 Join bookauthors ba ON ba.book_id = b.id" +
            "                 Join authors a ON a.id = ba.author_id " +
            "                 where b.publication_year = @PublicationYear" +
            "                 Order by b.id";


        var books = new Dictionary<int, BookWithAuthorsDTO>();

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(query, _connection, _transaction());
        command.Parameters.Add(new SqlParameter("PublicationYear", year));
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal("id");
        var titleOrd = reader.GetOrdinal("title");
        var yearOrd = reader.GetOrdinal("publication_year");
        var authorIdOrd = reader.GetOrdinal("author_id");
        var nameOrd = reader.GetOrdinal("name");
        var surnameOrd = reader.GetOrdinal("surname");

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
