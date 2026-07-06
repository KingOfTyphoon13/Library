using Library.Domain.DataAccess;
using Library.Domain.DTOs.Books;
using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal class BooksRepository : BaseRepository, IBooksRepository
{
    public BooksRepository(SqlConnection connection, Func<SqlTransaction?> transaction) : base(connection, transaction)
    {
    }

    public Task<int> AddAsync(CreateBookDTO dto)
    {
        throw new NotImplementedException();
    }

    public Task<List<BookWithAuthorsDTO>> GetBooksAsync()
    {
        throw new NotImplementedException();
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
