using PlastMB.Application.CustomModels.Dtos;
using PlastMB.Application.CustomModels.SearchConditions;
using PlastMB.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlastMB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MstFactoryController : BaseController
    {
        #region Properties
        private readonly IMstFactoryService _service;
        #endregion
        #region Constructor
        public MstFactoryController(IMstFactoryService service)
        {
            _service = service;
        }
        #endregion
        #region Search
        [AllowAnonymous]
        [HttpPost("search-MstFactory")]
        public async Task<IActionResult> SearchMstFactory(MstFactorySearchImpl condition)
        {
            var result = await _service.SearchMstFactory(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD

        #endregion
    }
}
