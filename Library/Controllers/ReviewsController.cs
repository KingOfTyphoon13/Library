using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers;

public class ReviewsController : BaseController
{
    public IActionResult Index()
    {
        return View();
    }
}
