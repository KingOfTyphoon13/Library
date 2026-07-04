using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class BooksController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
