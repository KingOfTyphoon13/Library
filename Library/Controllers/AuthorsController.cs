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

    public IActionResult Index()
    {
        var authors = _authorsService.GetAuthorWithBooksCounts()
                            .Select(a => _mapper.Map<AuthorListItemViewModel>(a))
                            .ToList();

        var model = new AuthorsIndexViewModel { Authors = authors };

        return View(model);
    }
}
