using Core.Application.CustomModels.Others;
using Core.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HandyController : BaseController
    {

        #region Properties
        private readonly IHandyService _service;
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public HandyController(IHandyService service)
        {
            _service = service;
        }
        #endregion
        #region Others
        [HttpPost("check-handy-info")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckHandyInfo(HandyInfo data)
        {
            var result = await _service.CheckHandyInfo(data);
            return new ObjectResult(result);
        }
        #endregion
    }
}
