using Library.DAL.QueryBuilder.Directors;
using Library.Domain.Common.Pagination;
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
        var query = AuthorQueryDirector.Insert(dto);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());

        return (int)await command.ExecuteScalarAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var query = AuthorQueryDirector.ExistsById(id);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        var result = await command.ExecuteScalarAsync();
        return result is not null;
    }

    public async Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync(KeysetRequest request)
    {
        var query = AuthorQueryDirector.GetKeysetWithBookCount(request);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Id);
        var nameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Name);
        var surnameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Surname);
        var countOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.BooksCount);

        var result = new List<AuthorWithBooksCountDTO>();
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

    public async Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync(PagedRequest request)
    {
        var query = AuthorQueryDirector.GetPagedWithBookCount(request);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Id);
        var nameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Name);
        var surnameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Surname);
        var countOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.BooksCount);

        var result = new List<AuthorWithBooksCountDTO>();
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

    public async Task<List<AuthorDTO>> GetAuthors(KeysetRequest request)
    {
        var query = AuthorQueryDirector.GetKeyset(request);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Id);
        var nameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Name);
        var surnameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Surname);

        var result = new List<AuthorDTO>();
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

    public async Task<List<AuthorDTO>> GetAuthors(PagedRequest request)
    {
        var query = AuthorQueryDirector.GetPaged(request);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Id);
        var nameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Name);
        var surnameOrd = reader.GetOrdinal(AuthorQueryDirector.Columns.Surname);

        var result = new List<AuthorDTO>();
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

    public async Task<int> GetTotalEntries()
    {
        var query = AuthorQueryDirector.GetTotalCount();

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        return (int)await command.ExecuteScalarAsync();
    }
}
