using AutoMapper;
using Library.Domain.Services.AuthorsService;
using Library.ViewModels.Authors;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class AuthorsController : BaseController
{
    private readonly IAuthorsService _authorsService;
    public AuthorsController(IAuthorsService authorsService, IMapper mapper, ILogger<AuthorsController> logger) : base(mapper, logger)
    {
        _authorsService = authorsService;
    }

    public async Task<IActionResult> Index()
    {
        var authors = (await _authorsService.GetAuthorWithBooksCountsAsync())
                            .Select(a => _mapper.Map<AuthorListItemViewModel>(a))
                            .ToList();

        var model = new AuthorsIndexViewModel { Authors = authors };

        return View(model);
    }
}
