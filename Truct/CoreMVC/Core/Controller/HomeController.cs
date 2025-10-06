using Microsoft.AspNetCore.Mvc;

namespace Core.Controller
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
