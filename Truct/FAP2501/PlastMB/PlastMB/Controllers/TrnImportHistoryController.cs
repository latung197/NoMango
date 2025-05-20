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
    public class TrnImportHistoryController : BaseController
    {
        #region Properties
        private readonly ITrnImportHistoryService _service;
        #endregion
        #region Constructor
        public TrnImportHistoryController(ITrnImportHistoryService service)
        {
            _service = service;
        }
        #endregion
        #region Search
        [AllowAnonymous]
        [HttpPost("search-History")]
        public async Task<IActionResult> SearchImportHistory(TrnImportHistorySearchImpl condition)
        {
            var result = await _service.SearchImportHistory(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD
        [AllowAnonymous]
        [HttpPost("import-list-History")]
        public async Task<IActionResult> ImportListImportHistory(List<TrnImportHistoryDto> data)
        {
            var result = await _service.ImportListImportHistory(data);
            return new ObjectResult(result);
        }


        #endregion
    }
}
