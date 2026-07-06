using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal class AuthorsRepository : BaseRepository, IAuthorsRepository
{
    public AuthorsRepository(SqlConnection connection, Func<SqlTransaction?> transaction) : base(connection, transaction)
    {
    }

    public Task<int> AddAsync(AuthorDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<AuthorWithBooksCountDTO>> GetAllWithBookCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<AuthorDTO>> GetAuthors()
    {
        throw new NotImplementedException();
    }

    public Task<AuthorDTO?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
