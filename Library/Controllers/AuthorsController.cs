using AutoMapper;
using Library.Domain.Common.Pagination;
using Library.Domain.Services.AuthorsService;
using Library.ViewModels.Authors;
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

        var model = new AuthorsIndexViewModel
        {
            Authors = result.Items.Select(_mapper.Map<AuthorListItemViewModel>).ToList()
        };

        return View(model);
    }
}
