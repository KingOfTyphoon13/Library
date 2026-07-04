namespace Library.ViewModels.Books;

public class BookReviewStatsItemViewModel : BaseBookViewModel
{
    public int ReviewCount { get; set; }
    public double AverageScore { get; set; }
}

public class BooksByMinReviewsViewModel
{
    public int? MinReviewCount { get; set; }

    public List<BookReviewStatsItemViewModel> Results { get; set; } = [];
}
