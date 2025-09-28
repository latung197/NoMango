using Microsoft.AspNetCore.Mvc;

namespace CoreMVC.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
