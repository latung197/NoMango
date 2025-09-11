using Microsoft.AspNetCore.Mvc;

namespace PuduIOT.Controllers
{
    public class PuduApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
