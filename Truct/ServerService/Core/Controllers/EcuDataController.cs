using Core.Application.CustomModels.Dtos;
using Core.Application.CustomModels.SearchConditions;
using Core.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EcuDataController : BaseController
    {
        #region Properties
        private readonly IEcuDataService _service;
        #endregion
        #region Constructor
        public EcuDataController(IEcuDataService service)
        {
            _service = service;
        }
        #endregion
        #region Search
        [HttpPost("search-ecu-data")]
        public async Task<IActionResult> SearchEcuData(EcuDataSearchImpl condition)
        {
            var result = await _service.SearchEcuData(condition);
            return new ObjectResult(result);
        }
        #endregion
        #region CRUD
        [AllowAnonymous]
        [HttpPost("import-list-ecu-data")]
        public async Task<IActionResult> ImportListEcuData(List<EcuDataDto> data)
        {
            var result = await _service.ImportListEcuData(data);
            return new ObjectResult(result);
        }
        [HttpPost("update-ecu-data")]
        public async Task<IActionResult> UpdateEcuData(EcuDataDto dto)
        {
            var result = await _service.UpdateEcuData(dto);
            return new ObjectResult(result);
        }
        [HttpDelete("delete-ecu-data")]
        public async Task<IActionResult> DeleteEcuData(int id)
        {
            var result = await _service.DeleteEcuData(id);
            return new ObjectResult(result);
        }
        #endregion
    }
}
