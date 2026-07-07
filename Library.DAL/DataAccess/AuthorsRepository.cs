using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal class AuthorsRepository : BaseRepository, IAuthorsRepository
{
    public AuthorsRepository(SqlConnection connection, Func<SqlTransaction?> transaction, Func<Task?> openDbConnection) : base(connection, transaction, openDbConnection)
    {
    }

    public async Task<int> AddAsync(AuthorDTO dto)
    {
        const string query = @"
        Insert into authors (name, surname) 
        Output Inserted.id
        Values(@Name, @Surname)";

        var cmd = new SqlCommand(query, _connection, _transaction());
        cmd.Parameters.Add(new SqlParameter("@Name", dto.Name));
        cmd.Parameters.Add(new SqlParameter("@Surname", dto.Surname));

        return (int)await cmd.ExecuteScalarAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        const string sql = "SELECT 1 FROM authors WHERE id = @Id";

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(sql, _connection, _transaction());
        command.Parameters.Add(new SqlParameter("@Id", id));

        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync()
    {
        const string query = @"
        SELECT a.id, a.name, a.surname,
               Count(b.id) as books_count
        FROM authors a
        LEFT JOIN bookauthors ba ON ba.author_id = a.id
        LEFT JOIN books b ON b.id = ba.book_id
        GROUP BY a.id, a.name, a.surname
        ORDER BY a.id";

        var result = new List<AuthorWithBooksCountDTO>();

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(query, _connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal("id");
        var nameOrd = reader.GetOrdinal("name");
        var surnameOrd = reader.GetOrdinal("surname");
        var countOrd = reader.GetOrdinal("books_count");

        while (await reader.ReadAsync())
        {
            result.Add(new AuthorWithBooksCountDTO
            {
                Id = reader.GetInt32(idOrd),
                Name = reader.GetString(nameOrd),
                Surname = reader.GetString(surnameOrd),
                Count = reader.GetInt32(countOrd)
            });
        }

        return result;
    }

    public async Task<List<AuthorDTO>> GetAuthors()
    {
        const string sql = @"
        SELECT a.id, a.name, a.surname
        FROM authors a
        ORDER BY a.id";

        var result = new List<AuthorDTO>();

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(sql, _connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal("id");
        var nameOrd = reader.GetOrdinal("name");
        var surnameOrd = reader.GetOrdinal("surname");

        while (await reader.ReadAsync())
        {
            result.Add(new AuthorDTO
            {
                Id = reader.GetInt32(idOrd),
                Name = reader.GetString(nameOrd),
                Surname = reader.GetString(surnameOrd),
            });
        }

        return result;
    }
}
