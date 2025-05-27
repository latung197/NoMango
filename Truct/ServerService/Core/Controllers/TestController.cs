using Core.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : BaseController
    {
        #region Properties
        private readonly ITestService _service;
        #endregion
        #region Constructor
        public TestController(ITestService service)
        {
            _service = service;
        }

        #endregion
        [AllowAnonymous]
        [HttpGet("index")]
        public IActionResult Index(Dictionary<int, string> data)
        {
            var result = _service.TestDict();
            return new ObjectResult(result);
        }
    }
}
