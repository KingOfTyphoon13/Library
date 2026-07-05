using Library.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class BooksController : BaseController
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
        new() { Id = 6, Title = "Dune Messiah", PublicationYear = 1969,
            Authors = [new() { Id = 5, Name = "Frank", Surname = "Herbert" }] },
    ];

    public IActionResult Index(int? publicationYear)
    {
        var books = GetDummyBooks();

        var model = new BooksByYearViewModel
        {
            PublicationYear = publicationYear,
            Results = publicationYear.HasValue
                ? books.Where(b => b.PublicationYear == publicationYear.Value).ToList()
                : books
        };

        return View(model);
    }
}
