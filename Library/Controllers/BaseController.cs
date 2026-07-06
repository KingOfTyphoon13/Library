using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public abstract class BaseController : Controller
{
    protected readonly ILogger<BaseController> _logger;
    protected readonly IMapper _mapper;

    protected BaseController(IMapper mapper, ILogger<BaseController> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}
