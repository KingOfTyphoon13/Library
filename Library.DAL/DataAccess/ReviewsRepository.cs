using Library.Domain.DataAccess;
using Library.Domain.DTOs.Reviews;
using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal class ReviewsRepository : BaseRepository, IReviewsRepository
{
    public ReviewsRepository(SqlConnection connection, Func<SqlTransaction?> transaction) : base(connection, transaction)
    {
    }

    public Task<List<ReviewDTO>> GetReviewsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> AddAsync(ReviewDTO dto)
    {
        throw new NotImplementedException();
    }
}
