using Library.Domain.DataAccess;
using Library.Domain.DTOs.Authors;
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
        SELECT 
            r.id, r.book_id, r.score,
            b.title, b.publication_year,
            a.id AS author_id, a.name, a.surname
        FROM reviews r
        JOIN books b ON b.id = r.book_id
        JOIN bookauthors ba ON ba.book_id = b.id
        JOIN authors a ON a.id = ba.author_id
        ORDER BY r.id, b.id";

        var result = new List<ReviewWithBookInfoDTO>();
        var reviewDict = new Dictionary<int, ReviewWithBookInfoDTO>();

        await _openDbConnectionAsync();

        await using var command = new SqlCommand(sql, _connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal("id");
        var bookIdOrd = reader.GetOrdinal("book_id");
        var scoreOrd = reader.GetOrdinal("score");
        var titleOrd = reader.GetOrdinal("title");
        var yearOrd = reader.GetOrdinal("publication_year");
        var authorIdOrd = reader.GetOrdinal("author_id");
        var nameOrd = reader.GetOrdinal("name");
        var surnameOrd = reader.GetOrdinal("surname");

        while (await reader.ReadAsync())
        {
            var reviewId = reader.GetInt32(idOrd);

            if (!reviewDict.TryGetValue(reviewId, out var review))
            {
                review = new ReviewWithBookInfoDTO
                {
                    Id = reviewId,
                    BookId = reader.GetInt32(bookIdOrd),
                    Score = reader.GetInt32(scoreOrd),
                    BookInfo = new BookWithAuthorsDTO
                    {
                        Id = reader.GetInt32(bookIdOrd),
                        Title = reader.GetString(titleOrd),
                        PublicationYear = reader.GetInt32(yearOrd),
                        Authors = []
                    }
                };

                reviewDict[reviewId] = review;
                result.Add(review);
            }

            var author = new AuthorDTO
            {
                Id = reader.GetInt32(authorIdOrd),
                Name = reader.GetString(nameOrd),
                Surname = reader.GetString(surnameOrd)
            };

            review.BookInfo.Authors.Add(author);
        }

        return result;
    }

    public async Task<int> AddAsync(ReviewDTO dto)
    {
        const string query = "Insert Into reviews (book_id, score) " +
                             "Output Inserted.Id " +
                             "Values (@BookID, @Score) ";

        var cmd = new SqlCommand(query, _connection, _transaction());
        cmd.Parameters.Add(new SqlParameter("@BookID", dto.BookId));
        cmd.Parameters.Add(new SqlParameter("@Score", dto.Score));

        return (int)await cmd.ExecuteScalarAsync();
    }
}
