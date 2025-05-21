using Astemo.Application.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Astemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : BaseController
    {
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok("Hệ thống quản lý xuất hàng");
        }

        [HttpGet("version")]
        public IActionResult Ping()
        {
            return new ObjectResult($"Hệ thống quản lý xuất hàng{Environment.NewLine}{CommonConstant.VERSION_CODE}{Environment.NewLine}{CommonConstant.VERSION_DATE}");
        }
    }
}
