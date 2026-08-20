using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class StageController(StageService stageService, ILogger<StageController> logger) : ControllerBase
{
    private readonly StageService _stageService = stageService;
    private readonly ILogger<StageController> _logger = logger;

    /// <summary>
    /// Hiển thị danh sách stage
    /// </summary>
    [HttpGet("stages")]
    //public async Task<ActionResult<BaseResponse<IEnumerable<StageResponse>>>> List([FromQuery] StageListRequest request)
    public async Task<ActionResult<BaseResponse<IEnumerable<StageResponse>>>> List()
    {
        var stages = await _stageService.GetListAsync();
        return Ok(BaseResponse<IEnumerable<StageResponse>>.SuccessResult(
            [.. stages.Select(StageResponse.FromStage)],
            "Lấy danh sách Stage thành công",
            new Meta { Total = stages.Count() }
        ));
    }

    [HttpGet("stages/area")]
    public async Task<ActionResult<BaseResponse<IEnumerable<StageResponse>>>> ListByArea([FromQuery] StageByAreaListRequest request)
    {
        var stages = await _stageService.GetListByAreaAsync(request);
        return Ok(BaseResponse<IEnumerable<StageResponse>>.SuccessResult(
            [.. stages.Select(StageResponse.FromStage)],
            "Lấy danh sách Stage thành công"
        ));
    }

    /// <summary>
    /// Hiển thị stage theo code
    /// </summary>
    [HttpGet("stages/{id}")]
    public async Task<ActionResult<BaseResponse<StageResponse>>> Show(string id)
    {
        var stage = await _stageService.ShowStageAsync(id);
        return Ok(BaseResponse<StageResponse>.SuccessResult(StageResponse.FromStage(stage), "Lấy thông tin Stage thành công"));
    }

    /// <summary>
    /// Tạo stage mới
    /// </summary>
    [HttpPost("stages")]
    public async Task<ActionResult<BaseResponse<StageResponse>>> Create(StageCreateRequest request)
    {
        var stage = await _stageService.CreateStageAsync(request);
        return Ok(BaseResponse<StageResponse>.SuccessResult(StageResponse.FromStage(stage), "Tạo stage thành công"));
    }

    /// <summary>
    /// Cập nhật stage
    /// </summary>
    [HttpPut("stages/{id}")]
    public async Task<ActionResult<BaseResponse<StageResponse>>> Update(string id, StageUpdateRequest request)
    {
        var stage = await _stageService.UpdateStageAsync(id, request);
        return Ok(BaseResponse<StageResponse>.SuccessResult(StageResponse.FromStage(stage), "Cập nhật stage thành công"));
    }

    /// <summary>
    /// Xóa stage (soft delete)
    /// </summary>
    [HttpDelete("stages/{id}")]
    public async Task<ActionResult<BaseResponse<object>>> Delete(string id)
    {
        await _stageService.DeleteStageAsync(id);
        return Ok(BaseResponse<object>.SuccessResult("Xóa stage thành công"));
    }
}

