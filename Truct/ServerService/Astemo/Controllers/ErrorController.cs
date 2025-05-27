using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorController : BaseController
    {
        public IActionResult Index()
        {
            return new ObjectResult("Đã có lỗi xảy ra!");
        }
    }
}
