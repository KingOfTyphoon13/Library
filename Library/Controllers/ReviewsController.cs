using AutoMapper;
using Library.Domain.Common.Pagination;
using Library.Domain.DTOs.Reviews;
using Library.Domain.Services.BooksService;
using Library.Domain.Services.ReviewService;
using Library.ViewModels.Books;
using Library.ViewModels.Common.Pagination;
using Library.ViewModels.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class ReviewsController : BaseController
{
    private readonly IReviewService _reviewService;
    private readonly IBooksService _bookService;

    public ReviewsController(IReviewService reviewService, IBooksService bookService, IMapper mapper, ILogger<ReviewsController> logger) : base(mapper, logger)
    {
        _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
        _bookService = bookService ?? throw new ArgumentNullException(nameof(_bookService));
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var request = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _reviewService.GetReviewsAsync(request);

        var paginatedModel = new PaginatedViewModel<ReviewListItemViewModel>
        {
            Items = result.Items.Select(_mapper.Map<ReviewListItemViewModel>).ToList(),
            Pagination = new PaginationViewModel
            {
                PageNumber = result.PageNumber,
                TotalPages = result.TotalPages,
                PageSize = result.PageSize,
                ActionName = "Index",
                ControllerName = "Reviews",
                ContainerId = "paged-content"
            }
        };

        var model = new ReviewsIndexViewModel() { RecentReviews = paginatedModel };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_PaginatedContent", model);
        }

        return View(model);
    }

    public async Task<IActionResult> Create(int? bookId)
    {
        var bookNumbers = await _bookService.GetBooksNumberAsync();
        var model = new ReviewCreateViewModel();

        if (bookId is not null)
        {
            var book = await _bookService.GetBookByIdAsync(bookId.Value);
            model.Book = book is null ? null : _mapper.Map<BookSummaryViewModel>(book);
            model.BookId = model.Book?.Id ?? 0;
        }
        else
        {
            model.AvailableBooks = (await _bookService.GetBooksWithAuthorsAsync(new PagedRequest() { PageNumber = 1, PageSize = bookNumbers })).Items
                .Select(_mapper.Map<BookSummaryViewModel>)
                .ToList();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewCreateViewModel model)
    {
        var book = await _bookService.GetBookByIdAsync(model.BookId);
        var bookNumbers = await _bookService.GetBooksNumberAsync();
        model.Book = book is null ? null : _mapper.Map<BookSummaryViewModel>(book);

        if (!ModelState.IsValid || model.Book is null)
        {
            if (model.Book is null)
                model.AvailableBooks = (await _bookService.GetBooksWithAuthorsAsync(new PagedRequest() { PageNumber = 1, PageSize = bookNumbers })).Items
                    .Select(_mapper.Map<BookSummaryViewModel>)
                    .ToList();
            return View(model);
        }

        await _reviewService.SaveReviewAsync(_mapper.Map<ReviewDTO>(model));
        return RedirectToAction(nameof(Index));
    }
}
