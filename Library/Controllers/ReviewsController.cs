using Library.ViewModels.Books;
using Library.ViewModels.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class ReviewsController : BaseController
{
    private static List<BookSummaryViewModel> GetDummyBooks() =>
    [
        new() { Id = 1, Title = "The Silent Patient", PublicationYear = 2019,
            Authors = [new() { Id = 1, Name = "Alex", Surname = "Michaelides" }] },
        new() { Id = 2, Title = "Clean Code", PublicationYear = 2008,
            Authors = [new() { Id = 2, Name = "Robert", Surname = "Martin" }] },
        new() { Id = 3, Title = "Good Omens", PublicationYear = 1990,
            Authors = [
                new() { Id = 3, Name = "Terry", Surname = "Pratchett" },
                new() { Id = 4, Name = "Neil", Surname = "Gaiman" } ] },
        new() { Id = 4, Title = "Dune", PublicationYear = 1965,
            Authors = [new() { Id = 5, Name = "Frank", Surname = "Herbert" }] },
        new() { Id = 5, Title = "Project Hail Mary", PublicationYear = 2021,
            Authors = [new() { Id = 6, Name = "Andy", Surname = "Weir" }] },
    ];

    private static List<ReviewListItemViewModel> GetReviews()
    {
        var books = GetDummyBooks();
        var rnd = new Random();

        var reviews = Enumerable.Range(1, 10).Select(i =>
        {
            var book = books[rnd.Next(books.Count)];
            return new ReviewListItemViewModel
            {
                Id = i,
                BookId = book.Id,
                Score = rnd.Next(1, 11),
                BookSummary = book
            };
        }).ToList();

        return reviews;
    }

    private static List<ReviewListItemViewModel> _reviews = GetReviews();

    public IActionResult Index()
    {

        return View(new ReviewsIndexViewModel { RecentReviews = _reviews });
    }

    public IActionResult Create(int? bookId)
    {
        var books = GetDummyBooks();
        var model = new ReviewCreateViewModel();

        if (bookId is not null)
        {
            model.Book = books.FirstOrDefault(b => b.Id == bookId);
            model.BookId = model.Book?.Id ?? 0;
        }
        else
        {
            model.AvailableBooks = books;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ReviewCreateViewModel model)
    {
        var books = GetDummyBooks();
        model.Book = books.FirstOrDefault(b => b.Id == model.BookId);

        if (!ModelState.IsValid)
        {
            if (model.Book is null)
                model.AvailableBooks = books;

            return View(model);
        }

        // TODO: persist via IReviewService once Service/DAL layers exist
        TempData["Success"] = "Review submitted.";

        _reviews.Add(new() { Id = _reviews.Count, BookId = model.BookId, BookSummary = model.Book, Score = model.Score });

        return RedirectToAction(nameof(Index));
    }
}
