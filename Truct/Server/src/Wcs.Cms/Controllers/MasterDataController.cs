using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Common.ValueObjects;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class MasterDataController(ILogger<MasterDataController> logger) : ControllerBase
{
    private readonly ILogger<MasterDataController> _logger = logger;

    [HttpGet("master-data")]
    public ActionResult<BaseResponse<MasterDataResponse>> GetMasterData()
    {
        return Ok(BaseResponse<MasterDataResponse>.SuccessResult(new MasterDataResponse { 
            Areas = StageArea.GetAll()
        }, "Lấy dữ liệu master data thành công"));
    }
}