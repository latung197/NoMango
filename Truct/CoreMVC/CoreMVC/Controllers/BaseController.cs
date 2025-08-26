using CoreMVC.BL.Interfaces;
using CoreMVC.DA.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace CoreMVC.Controllers
{
    [KeepAlive]
    public class BaseController : Controller
    {
        private readonly ILanguageService _languageService;
       // private readonly IMstConfigurationService _configurationService;

        public IActionResult Index()
        {
            return View();
        }
    }
}
