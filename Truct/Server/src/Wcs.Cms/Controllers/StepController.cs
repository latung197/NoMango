using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class StepController(StepService stepService, ILogger<StepController> logger) : ControllerBase
{
    private readonly StepService _stepService = stepService;
    private readonly ILogger<StepController> _logger = logger;

    /// <summary>
    /// Hiển thị danh sách step
    /// </summary>
    [HttpGet("steps")]
    public async Task<ActionResult<BaseResponse<IEnumerable<StepResponse>>>> List([FromQuery] StepListRequest request)
    {
        var steps = await _stepService.GetListAsync(request);
        return Ok(BaseResponse<IEnumerable<StepResponse>>.SuccessResult(
            [.. steps.Select(StepResponse.FromStep)],
            "Lấy danh sách Step thành công",
            new Meta { Total = steps.Count(), CurrentPage = request.PageNumber, Size = request.PageSize }
        ));
    }

    /// <summary>
    /// Hiển thị step theo code
    /// </summary>
    [HttpGet("steps/{id}")]
    public async Task<ActionResult<BaseResponse<StepResponse>>> Show(string id)
    {
        var step = await _stepService.ShowStepAsync(id);
        return Ok(BaseResponse<StepResponse>.SuccessResult(StepResponse.FromStep(step), "Lấy thông tin Step thành công"));
    }

    /// <summary>
    /// Tạo step mới
    /// </summary>
    [HttpPost("steps")]
    public async Task<ActionResult<BaseResponse<StepResponse>>> Create(StepCreateRequest request)
    {
        var step = await _stepService.CreateStepAsync(request);
        return Ok(BaseResponse<StepResponse>.SuccessResult(StepResponse.FromStep(step), "Tạo Step thành công"));
    }

    /// <summary>
    /// Cập nhật step
    /// </summary>
    [HttpPut("steps/{id}")]
    public async Task<ActionResult<BaseResponse<StepResponse>>> Update(string id, StepUpdateRequest request)
    {
        var step = await _stepService.UpdateStepAsync(id, request);
        return Ok(BaseResponse<StepResponse>.SuccessResult(StepResponse.FromStep(step), "Cập nhật Step thành công"));
    }
}

