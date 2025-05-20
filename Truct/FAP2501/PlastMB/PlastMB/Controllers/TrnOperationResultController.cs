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
    public class TrnOperationResultController : BaseController
    {
        #region Properties
        private readonly ITrnOperationResultService _service;
        #endregion
        #region Constructor
        public TrnOperationResultController(ITrnOperationResultService service)
        {
            _service = service;
        }
        #endregion
        #region Search
        [AllowAnonymous]
        [HttpPost("search-TrnOperationResult")]
        public async Task<IActionResult> SearchTrnOperationResult(TrnOperationResultSearchImpl condition)
        {
            var result = await _service.SearchTrnOperationResult(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD
        [AllowAnonymous]
        [HttpPost("import-list-TrnOperationResult")]
        public async Task<IActionResult> ImportListTrnOperationResult(List<TrnOperationResultDto> data)
        {
            var result = await _service.ImportListTrnOperationResult(data);
            return new ObjectResult(result);
        }

        #endregion
    }
}
