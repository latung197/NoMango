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
    public class MstMachineController : BaseController
    {
        #region Properties
        private readonly IMstMachineService _service;
        #endregion
        #region Constructor
        public MstMachineController(IMstMachineService service)
        {
            _service = service;
        }
        #endregion
        #region Search
        [AllowAnonymous]
        [HttpPost("search-MstMachine")]
        public async Task<IActionResult> SearchMstMachine(MstMachineSearchImpl condition)
        {
            var result = await _service.SearchMstMachine(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD

        #endregion
    }
}
