using AutoMapper;
using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Books;
using Library.Domain.Services.AuthorsService;
using Library.Domain.Services.BooksService;
using Library.ViewModels.Authors;
using Library.ViewModels.Books;
using Library.ViewModels.Common.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers;

public class BooksController : BaseController
{
    private readonly IBooksService _bookService;
    private readonly IAuthorsService _authorsService;

    private const int DefaultPageSize = 10;

    public BooksController(IBooksService bookService, IAuthorsService authorsService, IMapper mapper, ILogger<BaseController> logger) : base(mapper, logger)
    {
        _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
        _authorsService = authorsService ?? throw new ArgumentNullException(nameof(authorsService));
    }

    public async Task<IActionResult> Index(int? publicationYear, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        var result = await _bookService.GetBooksWithAuthorsAsync(
            new PagedRequest { PageNumber = pageNumber, PageSize = pageSize },
            publicationYear);

        var model = new BooksByYearViewModel
        {
            PublicationYear = publicationYear,
            Paginated = new PaginatedViewModel<BookSummaryViewModel>
            {
                Items = result.Items.Select(_mapper.Map<BookSummaryViewModel>).ToList(),
                Pagination = new PaginationViewModel
                {
                    PageNumber = result.PageNumber,
                    TotalPages = result.TotalPages,
                    PageSize = result.PageSize,
                    ActionName = nameof(Index),
                    ControllerName = "Books",
                    ContainerId = "booksContainer"
                }
            }
        };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_BooksPaginatedContent", model.Paginated);

        return View(model);
    }

    public async Task<IActionResult> ByMinReviews(int? minReviewCount, int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        var result = await _bookService.GetBookWithMinReviewCountAsync(
            new PagedRequest { PageNumber = pageNumber, PageSize = pageSize },
            minReviewCount);

        var model = new BooksByMinReviewsViewModel
        {
            MinReviewCount = minReviewCount,
            Paginated = new PaginatedViewModel<BookReviewStatsItemViewModel>
            {
                Items = result.Items.Select(_mapper.Map<BookReviewStatsItemViewModel>).ToList(),
                Pagination = new PaginationViewModel
                {
                    PageNumber = result.PageNumber,
                    TotalPages = result.TotalPages,
                    PageSize = result.PageSize,
                    ActionName = nameof(ByMinReviews),
                    ControllerName = "Books",
                    ContainerId = "booksByMinReviewsContainer"
                }
            }
        };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_BooksByMinReviewsPaginatedContent", model.Paginated);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new BookCreateViewModel
        {
            Authors = [new AuthorSlotViewModel()],
            AvailableAuthors = await GetAuthorsAsync()
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
            await _bookService.AddBookAsync(dto);
        }
        catch (ValidationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.AvailableAuthors = await GetAuthorsAsync();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<AuthorViewModel>> GetAuthorsAsync()
    {
        var authorsNumber = await _authorsService.GetAuthorsNumberAsync();

        var result = await _authorsService.GetAuthorsAsync(
            new PagedRequest { PageNumber = 1, PageSize = authorsNumber });

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
