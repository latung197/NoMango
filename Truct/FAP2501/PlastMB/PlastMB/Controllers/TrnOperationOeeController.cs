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
    public class TrnOperationOeeController : BaseController
    {
        #region Properties
        private readonly ITrnOperationOeeService _service;
        #endregion
        #region Constructor
        public TrnOperationOeeController(ITrnOperationOeeService service)
        {
            _service = service;
        }
        #endregion
        #region Search
        [AllowAnonymous]
        [HttpPost("search-TrnOperationOee")]
        public async Task<IActionResult> SearchTrnOperationOee(TrnOperationOeeSearchImpl condition)
        {
            var result = await _service.SearchTrnOperationOee(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD
        [AllowAnonymous]
        [HttpPost("import-list-TrnOperationOee")]
        public async Task<IActionResult> ImportListTrnOperationOee(List<TrnOperationOeeDto> data)
        {
            var result = await _service.ImportListTrnOperationOee(data);
            return new ObjectResult(result);
        }
        //[HttpPost("update-ecu-data")]
        //public async Task<IActionResult> UpdateEcuData(EcuDataDto dto)
        //{
        //    var result = await _service.UpdateEcuData(dto);
        //    return new ObjectResult(result);
        //}
        //[HttpDelete("delete-ecu-data")]
        //public async Task<IActionResult> DeleteEcuData(int id)
        //{
        //    var result = await _service.DeleteEcuData(id);
        //    return new ObjectResult(result);
        //}
        #endregion
    }
}
