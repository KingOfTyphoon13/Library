using Library.ViewModels.Authors;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class AuthorsController : BaseController
{
    private static List<AuthorListItemViewModel> GetDummyAuthors() =>
    [
       new() { Id = 1, Name = "Alex", Surname = "Michaelides", BookCount = 1 },
        new() { Id = 2, Name = "Robert", Surname = "Martin", BookCount = 3 },
        new() { Id = 3, Name = "Terry", Surname = "Pratchett", BookCount = 5 },
        new() { Id = 4, Name = "Neil", Surname = "Gaiman", BookCount = 4 },
        new() { Id = 5, Name = "Frank", Surname = "Herbert", BookCount = 2 },
        new() { Id = 6, Name = "Andy", Surname = "Weir", BookCount = 1 },
    ];

    public IActionResult Index()
    {
        var model = new AuthorsIndexViewModel { Authors = GetDummyAuthors() };
        return View(model);
    }
}
