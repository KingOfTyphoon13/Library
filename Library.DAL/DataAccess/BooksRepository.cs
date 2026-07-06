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


        var result = new List<BookWithAuthorsDTO>();

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(query, _connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var id = reader.GetOrdinal("id");
        var titleOrd = reader.GetOrdinal("title");
        var yearOrd = reader.GetOrdinal("publication_year");
        var authorIdOrd = reader.GetOrdinal("author_id");
        var nameOrd = reader.GetOrdinal("name");
        var surnameOrd = reader.GetOrdinal("surname");

        while (await reader.ReadAsync())
        {
            var dto = new BookWithAuthorsDTO()
            {
                Id = reader.GetInt32(id),
                Title = reader.GetString(titleOrd),
                PublicationYear = reader.GetInt32(yearOrd),

            };

            dto.Authors.Add(new AuthorDTO()
            {
                Id = reader.GetInt32(authorIdOrd),
                Name = reader.GetString(nameOrd),
                Surname = reader.GetString(surnameOrd),
            });

            result.Add(dto);
        }

        return result;
    }

    public Task<BookDTO?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<BookReviewStatsDTO>> GetByMinReviewCountAsync(int? minReviews)
    {
        throw new NotImplementedException();
    }

    public Task<List<BookWithAuthorsDTO>> GetByPublicationYearAsync(int? year)
    {
        throw new NotImplementedException();
    }
}
