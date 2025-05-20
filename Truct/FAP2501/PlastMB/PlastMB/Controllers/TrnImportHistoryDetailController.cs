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
    public class TrnImportHistoryDetailController : BaseController
    {
        #region Properties
        private readonly ITrnImportHistoryDetailService _service;
        #endregion
        #region Constructor
        public TrnImportHistoryDetailController(ITrnImportHistoryDetailService service)
        {
            _service = service;
        }
        #endregion
        #region Search
        [AllowAnonymous]
        [HttpPost("search-HistoryDetail")]
        public async Task<IActionResult> SearchImportHistoryDetail(TrnImportHistoryDetailSearchImpl condition)
        {
            var result = await _service.SearchImportHistoryDetail(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD
        [AllowAnonymous]
        [HttpPost("import-list-HistoryDetail")]
        public async Task<IActionResult> ImportListImportHistoryDetail(List<TrnImportHistoryDetailDto> data)
        {
            var result = await _service.ImportListImportHistoryDetail(data);
            return new ObjectResult(result);
        }


        #endregion
    }
}
