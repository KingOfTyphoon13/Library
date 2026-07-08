using Library.DAL.QueryBuilder.Directors;
using Library.Domain.Common.Pagination;
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

    public async Task<List<ReviewWithBookInfoDTO>> GetReviewsAsync(PagedRequest request)
    {
        var query = ReviewQueryDirector.GetPagedReviewsWithBookInfo(request);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        await using var reader = await command.ExecuteReaderAsync();

        var idOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.Id);
        var bookIdOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.BookId);
        var scoreOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.Score);
        var titleOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.Title);
        var yearOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.PublicationYear);
        var authorIdOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.AuthorId);
        var nameOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.Name);
        var surnameOrd = reader.GetOrdinal(ReviewQueryDirector.Columns.Surname);

        var result = new List<ReviewWithBookInfoDTO>();
        var reviewDict = new Dictionary<int, ReviewWithBookInfoDTO>();

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

            review.BookInfo.Authors.Add(new AuthorDTO
            {
                Id = reader.GetInt32(authorIdOrd),
                Name = reader.GetString(nameOrd),
                Surname = reader.GetString(surnameOrd)
            });
        }

        return result;
    }

    public async Task<int> GetTotalEntriesAsync()
    {
        var query = ReviewQueryDirector.GetTotalCount();

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        return (int)await command.ExecuteScalarAsync();
    }

    public async Task<int> AddAsync(ReviewDTO dto)
    {
        var query = ReviewQueryDirector.Insert(dto);

        await _openDbConnectionAsync();

        await using var command = query.ToCommand(_connection, _transaction());
        return (int)await command.ExecuteScalarAsync();
    }
}
