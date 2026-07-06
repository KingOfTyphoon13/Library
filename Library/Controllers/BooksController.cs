using AutoMapper;
using Library.Domain.DTOs.Books;
using Library.Domain.Services.AuthorsService;
using Library.Domain.Services.BooksService;
using Library.ViewModels.Authors;
using Library.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers;

public class BooksController : BaseController
{
    //    private static readonly List<BookSummaryViewModel> _books = [
    //        new() { Id = 1, Title = "The Silent Patient", PublicationYear = 2019,
    //            Authors = [new() { Id = 1, Name = "Alex", Surname = "Michaelides" }] },
    //        new() { Id = 2, Title = "Clean Code", PublicationYear = 2008,
    //            Authors = [new() { Id = 2, Name = "Robert", Surname = "Martin" }] },
    //        new() { Id = 3, Title = "Good Omens", PublicationYear = 1990,
    //            Authors = [
    //                new() { Id = 3, Name = "Terry", Surname = "Pratchett" },
    //                new() { Id = 4, Name = "Neil", Surname = "Gaiman" },
    //                new() { Id = 7, Name = "Kail", Surname = "Mosh" },
    //                new() { Id = 84, Name = "Diana", Surname = "Reavs" } ] },
    //        new () { Id = 4, Title = "Dune", PublicationYear = 1965,
    //            Authors = [new() { Id = 5, Name = "Frank", Surname = "Herbert" }] },
    //        new() { Id = 5, Title = "Project Hail Mary", PublicationYear = 2021,
    //            Authors = [new() { Id = 6, Name = "Andy", Surname = "Weir" }] },
    //        new() { Id = 6, Title = "Dune Messiah", PublicationYear = 1969,
    //            Authors = [new() { Id = 5, Name = "Frank", Surname = "Herbert" }] },
    //    ];

    //    private static readonly List<AuthorViewModel> _authors =
    //    [
    //        new() { Id = 1, Name = "Alex", Surname = "Michaelides" },
    //        new() { Id = 2, Name = "Robert", Surname = "Martin" },
    //        new() { Id = 3, Name = "Terry", Surname = "Pratchett" },
    //        new() { Id = 4, Name = "Neil", Surname = "Gaiman" },
    //        new() { Id = 5, Name = "Frank", Surname = "Herbert" },
    //        new() { Id = 6, Name = "Andy", Surname = "Weir" },
    //        new() { Id = 7, Name = "Kail", Surname = "Mosh" },
    //        new() { Id = 84, Name = "Diana", Surname = "Reavs" },
    //    ];

    //    private static List<BookReviewStatsItemViewModel> GetDummyBookStats() =>
    //    [
    //        new() { Id = 1, Title = "The Silent Patient", PublicationYear = 2019, ReviewCount = 12, AverageScore = 4.3 },
    //        new() { Id = 2, Title = "Clean Code", PublicationYear = 2008, ReviewCount = 30, AverageScore = 4.7 },
    //        new() { Id = 3, Title = "Good Omens", PublicationYear = 1990, ReviewCount = 8, AverageScore = 4.1 },
    //        new() { Id = 4, Title = "Dune", PublicationYear = 1965, ReviewCount = 45, AverageScore = 4.8 },
    //        new() { Id = 5, Title = "Project Hail Mary", PublicationYear = 2021, ReviewCount = 22, AverageScore = 4.6 },
    //        new() { Id = 6, Title = "Dune Messiah", PublicationYear = 1969, ReviewCount = 5, AverageScore = 3.9 },
    //];

    //    private static List<BookSummaryViewModel> GetDummyBooks() => _books;

    //    private static List<AuthorViewModel> GetDummyAuthors() => _authors;

    private readonly IBooksService _bookService;
    private readonly IAuthorsService _authorsService;

    public BooksController(IBooksService bookService, IAuthorsService authorsService, IMapper mapper, ILogger<BaseController> logger) : base(mapper, logger)
    {
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        _authorsService = authorsService ?? throw new ArgumentNullException(nameof(authorsService));
    }

    public async Task<IActionResult> Index(int? publicationYear)
    {
        var books = (await _bookService.GetBooksWithAuthorsAsync(publicationYear))
                        .Select(_mapper.Map<BookSummaryViewModel>)
                        .ToList();

        var model = new BooksByYearViewModel
        {
            PublicationYear = publicationYear,
            Results = books
        };

        return View(model);
    }

    public async Task<IActionResult> ByMinReviews(int? minReviewCount)
    {
        var books = (await _bookService.GetBookWithMinReviewCountAsync(minReviewCount))
                        .Select(_mapper.Map<BookReviewStatsItemViewModel>)
                        .ToList();

        var model = new BooksByMinReviewsViewModel
        {
            MinReviewCount = minReviewCount,
            Results = books
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var authors = await GetAuthors();

        var model = new BookCreateViewModel
        {
            Authors = [new AuthorSlotViewModel()],
            AvailableAuthors = authors
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookCreateViewModel model)
    {
        NormalizeAuthorSlots(model);

        if (!ModelState.IsValid)
        {
            model.AvailableAuthors = await GetAuthors();
            return View(model);
        }

        var dto = _mapper.Map<CreateBookDTO>(model);

        try
        {
            await _bookService.AddBook(dto);
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.AvailableAuthors = (await _authorsService.GetAuthorsAsync())
                .Select(_mapper.Map<AuthorViewModel>)
                .ToList();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<AuthorViewModel>> GetAuthors() => (await _authorsService.GetAuthorsAsync())
                .Select(_mapper.Map<AuthorViewModel>)
                .ToList();

    private void NormalizeAuthorSlots(BookCreateViewModel model)
    {
        for (var i = 0; i < model.Authors.Count; i++)
        {
            if (!model.Authors[i].ExistingAuthorId.HasValue)
            {
                continue;
            }

            model.Authors[i].NewAuthor = null;
            ModelState.Remove($"{nameof(model.Authors)}[{i}].{nameof(AuthorSlotViewModel.NewAuthor)}.Name");
            ModelState.Remove($"{nameof(model.Authors)}[{i}].{nameof(AuthorSlotViewModel.NewAuthor)}.Surname");
        }
    }
}
