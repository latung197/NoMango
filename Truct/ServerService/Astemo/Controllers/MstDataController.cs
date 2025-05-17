using Astemo.Application.CustomModels.Dtos;
using Astemo.Application.CustomModels.SearchConditions;
using Astemo.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Astemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MstDataController : BaseController
    {
        #region Properties
        private readonly IMstDataService _service;
        #endregion
        #region Constructor
        public MstDataController(IMstDataService service)
        {
            _service = service;
        }
        #endregion
        #region Search API
        [HttpPost("search-master-data")]
        public async Task<IActionResult> SearchMasterData(MstDataSearchImpl condition)
        {
            var result = await _service.SearchMasterData(condition, true);
            return new ObjectResult(result);
        }

        [HttpGet("get-master-data-by-id")]
        public async Task<IActionResult> GetMasterDataById(int id)
        {
            var result = await _service.GetMasterDataById(id);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD API
        [HttpPost("insert-list-data")]
        public async Task<IActionResult> InsertListData(List<MstDataDto> data)
        {
            var result = await _service.InsertListData(data);
            return new ObjectResult(result);
        }

        [HttpPost("update-list-data")]
        public async Task<IActionResult> UpdateListData(List<MstDataDto> data)
        {
            var result = await _service.UpdateListData(data);
            return new ObjectResult(result);
        }

        [HttpDelete("delete-master-data")]
        public async Task<IActionResult> DeleteMasterData(int id)
        {
            var result = await _service.DeleteMasterData(id);
            return new ObjectResult(result);
        }
        #endregion
    }
}
