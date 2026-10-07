using Microsoft.AspNetCore.Mvc;

namespace WebAssignment.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
