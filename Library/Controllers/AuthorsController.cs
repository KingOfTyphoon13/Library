using AutoMapper;
using Library.Domain.Common.Pagination;
using Library.Domain.Services.AuthorsService;
using Library.ViewModels.Authors;
using Library.ViewModels.Common.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class AuthorsController : BaseController
{
    private const int PageSize = 10;

    private readonly IAuthorsService _authorsService;
    public AuthorsController(IAuthorsService authorsService, IMapper mapper, ILogger<AuthorsController> logger) : base(mapper, logger)
    {
        _authorsService = authorsService;
    }

    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var request = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _authorsService.GetAuthorWithBooksCountsAsync(request);

        var paginatedModel = new PaginatedViewModel<AuthorListItemViewModel>
        {
            Items = result.Items.Select(_mapper.Map<AuthorListItemViewModel>).ToList(),
            Pagination = new PaginationViewModel
            {
                PageNumber = result.PageNumber,
                TotalPages = result.TotalPages,
                ActionName = "Index",
                ControllerName = "Authors"
            }
        };
        var model = new AuthorsIndexViewModel() { AuthorsListViewModel = paginatedModel };
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_PaginatedContent", model.AuthorsListViewModel);
        }

        return View(model);
    }
}
