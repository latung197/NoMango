using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class FlowController(FlowService flowService, ILogger<FlowController> logger) : ControllerBase
{
    private readonly FlowService _flowService = flowService;
    private readonly ILogger<FlowController> _logger = logger;

    /// <summary>
    /// Hiển thị danh sách flow
    /// </summary>
    [HttpGet("flows")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowResponse>>>> List([FromQuery] FlowListRequest request)
    {
        var flows = await _flowService.GetListAsync(request);
        var totalCount = await _flowService.CountAsync();
        return Ok(BaseResponse<IEnumerable<FlowResponse>>.SuccessResult(
            [.. flows.Select(FlowResponse.FromFlow)],
            "Lấy danh sách Flow thành công",
            new Meta { Total = totalCount, CurrentPage = request.PageNumber, Size = request.PageSize }
        ));
    }

    /// <summary>
    /// Tìm kiếm danh sách flow theo khu vực
    /// </summary>
    [HttpPost("flows/search")]
    public async Task<ActionResult<BaseResponse<IEnumerable<FlowStepResponse>>>> Search(FlowSearchRequest request)
    {
        var flows = await _flowService.GetListAsync(request);
        return Ok(BaseResponse<IEnumerable<FlowStepResponse>>.SuccessResult(
            [.. flows.Select(FlowStepResponse.FromFlow)],
            "Lấy danh sách Flow thành công",
            new Meta { Total = flows.Count() }
        ));
    }

    /// <summary>
    /// Hiển thị flow theo id
    /// </summary>
    [HttpGet("flows/{id}")]
    public async Task<ActionResult<BaseResponse<FlowResponse>>> Show(string id)
    {
        var flow = await _flowService.ShowFlowAsync(id);
        return Ok(BaseResponse<FlowResponse>.SuccessResult(FlowResponse.FromFlow(flow), "Lấy thông tin Flow thành công"));
    }

    /// <summary>
    /// Tạo flow mới
    /// </summary>
    [HttpPost("flows")]
    public async Task<ActionResult<BaseResponse<FlowResponse>>> Create(FlowCreateRequest request)
    {
        var flow = await _flowService.CreateFlowAsync(request);
        return Ok(BaseResponse<FlowResponse>.SuccessResult(FlowResponse.FromFlow(flow), "Tạo Flow thành công"));
    }

    /// <summary>
    /// Cập nhật flow
    /// </summary>
    [HttpPut("flows/{id}")]
    public async Task<ActionResult<BaseResponse<FlowResponse>>> Update(string id, FlowUpdateRequest request)
    {
        var flow = await _flowService.UpdateFlowAsync(id, request);
        return Ok(BaseResponse<FlowResponse>.SuccessResult(FlowResponse.FromFlow(flow), "Cập nhật Flow thành công"));
    }

    /// <summary>
    /// Xóa flow (soft delete)
    /// </summary>
    [HttpDelete("flows/{id}")]
    public async Task<ActionResult<BaseResponse<object>>> Delete(string id)
    {
        await _flowService.DeleteFlowAsync(id);
        return Ok(BaseResponse<object>.SuccessResult("Xóa Flow thành công"));
    }
}
