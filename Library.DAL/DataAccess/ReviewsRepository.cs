using Library.Domain.DataAccess;
using Library.Domain.DTOs.Books;
using Library.Domain.DTOs.Reviews;
using Microsoft.Data.SqlClient;

namespace Library.DAL.DataAccess;

internal class ReviewsRepository : BaseRepository, IReviewsRepository
{
    public ReviewsRepository(SqlConnection connection, Func<SqlTransaction?> transaction, Func<Task?> openDbConnection) : base(connection, transaction, openDbConnection)
    {
    }

    public async Task<List<ReviewWithBookInfoDTO>> GetReviewsAsync()
    {
        const string sql = @"
        SELECT r.id, r.book_id, r.score,
               b.title, b.publication_year
        FROM reviews r
        JOIN books b ON b.id = r.book_id
        ORDER BY r.id;";

        var result = new List<ReviewWithBookInfoDTO>();

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(sql, _connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal("id");
        var bookIdOrd = reader.GetOrdinal("book_id");
        var scoreOrd = reader.GetOrdinal("score");
        var titleOrd = reader.GetOrdinal("title");
        var yearOrd = reader.GetOrdinal("publication_year");

        while (await reader.ReadAsync())
        {
            result.Add(new ReviewWithBookInfoDTO
            {
                Id = reader.GetInt32(idOrd),
                BookId = reader.GetInt32(bookIdOrd),
                Score = reader.GetInt32(scoreOrd),
                BookInfo = new BookDTO
                {
                    Id = reader.GetInt32(bookIdOrd),
                    Title = reader.GetString(titleOrd),
                    PublicationYear = reader.GetInt32(yearOrd)
                }
            });
        }

        return result;
    }

    public Task<int> AddAsync(ReviewDTO dto)
    {
        throw new NotImplementedException();
    }
}
