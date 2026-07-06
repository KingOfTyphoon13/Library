namespace Library.Domain.DTOs.Books;

public class BookReviewStatsDTO : BookDTO
{
    public int ReviewCount { get; set; }
    public double AverageScore { get; set; }
}
