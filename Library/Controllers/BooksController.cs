using Library.ViewModels.Authors;
using Library.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class BooksController : BaseController
{
    private static readonly List<BookSummaryViewModel> _books = [
        new() { Id = 1, Title = "The Silent Patient", PublicationYear = 2019,
            Authors = [new() { Id = 1, Name = "Alex", Surname = "Michaelides" }] },
        new() { Id = 2, Title = "Clean Code", PublicationYear = 2008,
            Authors = [new() { Id = 2, Name = "Robert", Surname = "Martin" }] },
        new() { Id = 3, Title = "Good Omens", PublicationYear = 1990,
            Authors = [
                new() { Id = 3, Name = "Terry", Surname = "Pratchett" },
                new() { Id = 4, Name = "Neil", Surname = "Gaiman" },
                new() { Id = 7, Name = "Kail", Surname = "Mosh" },
                new() { Id = 84, Name = "Diana", Surname = "Reavs" } ] },
        new () { Id = 4, Title = "Dune", PublicationYear = 1965,
            Authors = [new() { Id = 5, Name = "Frank", Surname = "Herbert" }] },
        new() { Id = 5, Title = "Project Hail Mary", PublicationYear = 2021,
            Authors = [new() { Id = 6, Name = "Andy", Surname = "Weir" }] },
        new() { Id = 6, Title = "Dune Messiah", PublicationYear = 1969,
            Authors = [new() { Id = 5, Name = "Frank", Surname = "Herbert" }] },
    ];

    private static readonly List<AuthorViewModel> _authors =
    [
        new() { Id = 1, Name = "Alex", Surname = "Michaelides" },
        new() { Id = 2, Name = "Robert", Surname = "Martin" },
        new() { Id = 3, Name = "Terry", Surname = "Pratchett" },
        new() { Id = 4, Name = "Neil", Surname = "Gaiman" },
        new() { Id = 5, Name = "Frank", Surname = "Herbert" },
        new() { Id = 6, Name = "Andy", Surname = "Weir" },
        new() { Id = 7, Name = "Kail", Surname = "Mosh" },
        new() { Id = 84, Name = "Diana", Surname = "Reavs" },
    ];

    private static List<BookSummaryViewModel> GetDummyBooks() => _books;

    private static List<AuthorViewModel> GetDummyAuthors() => _authors;

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

    [HttpGet]
    public IActionResult Create()
    {
        var model = new BookCreateViewModel
        {
            Authors = [new AuthorSlotViewModel()],
            AvailableAuthors = GetDummyAuthors()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookCreateViewModel model)
    {
        NormalizeAuthorSlots(model);

        if (!ModelState.IsValid)
        {
            model.AvailableAuthors = GetDummyAuthors();
            return View(model);
        }

        var id = GetDummyBooks().Concat(_books).Select(b => b.Id).DefaultIfEmpty(0).Max() + 1;
        var availableAuthors = GetDummyAuthors();

        var resolvedAuthors = model.Authors.Select(slot => slot.ExistingAuthorId.HasValue
                ? availableAuthors.First(a => a.Id == slot.ExistingAuthorId.Value)
                : new AuthorViewModel { Id = 0, Name = slot.NewAuthor!.Name, Surname = slot.NewAuthor.Surname })
            .ToList();

        _books.Add(new BookSummaryViewModel
        {
            Id = id,
            Title = model.Title,
            PublicationYear = model.PublicationYear,
            Authors = resolvedAuthors
        });

        return RedirectToAction(nameof(Index));
    }

    private void NormalizeAuthorSlots(BookCreateViewModel model)
    {
        for (var i = 0; i < model.Authors.Count; i++)
        {
            if (!model.Authors[i].ExistingAuthorId.HasValue)
            {
                model.Authors[i].NewAuthor.Id = _authors.Last().Id++;
                _authors.Add(model.Authors[i].NewAuthor);
                continue;
            }

            model.Authors[i].NewAuthor = null;
            ModelState.Remove($"{nameof(model.Authors)}[{i}].{nameof(AuthorSlotViewModel.NewAuthor)}.Name");
            ModelState.Remove($"{nameof(model.Authors)}[{i}].{nameof(AuthorSlotViewModel.NewAuthor)}.Surname");
        }
    }
}
