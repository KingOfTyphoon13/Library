using AutoMapper;
using Library.Domain.Common.Pagination;
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
        var authors = await GetAuthorsAsync();

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
            model.AvailableAuthors = await GetAuthorsAsync();
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
            model.AvailableAuthors = (await GetAuthorsAsync())
                .Select(_mapper.Map<AuthorViewModel>)
                .ToList();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<AuthorViewModel>> GetAuthorsAsync()
    {
        var result = await _authorsService.GetAuthorsAsync(new PagedRequest
        {
            PageNumber = 1,
            PageSize = int.MaxValue
        });

        return result.Items.Select(_mapper.Map<AuthorViewModel>).ToList();
    }

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
