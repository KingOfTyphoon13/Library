using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class AuthorsController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
