using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Api.Services;

namespace Wcs.Api.Controllers;

/// <summary>
/// Tạm dừng / tiếp tục thực thi task mới trong WCS flow.
/// </summary>
[ApiController]
[Route("api/task-execution")]
public class TaskExecutionController(
    TaskExecutionGateService taskExecutionGateService,
    ILogger<TaskExecutionController> logger) : ControllerBase
{
    private readonly TaskExecutionGateService _taskExecutionGateService = taskExecutionGateService;
    private readonly ILogger<TaskExecutionController> _logger = logger;

    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TaskExecutionStatusDto>>> GetStatus(CancellationToken ct = default)
    {
        var status = await _taskExecutionGateService.GetStatusAsync(ct);
        return Ok(BaseResponse<TaskExecutionStatusDto>.SuccessResult(status, "Lấy trạng thái thực thi task thành công"));
    }

    [HttpPost("pause")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TaskExecutionStatusDto>>> Pause(
        [FromBody] TaskExecutionControlRequest? request = null,
        CancellationToken ct = default)
    {
        _logger.LogWarning("Nhận yêu cầu tạm dừng thực thi task mới. Reason={Reason}", request?.Reason);
        await _taskExecutionGateService.PauseAsync(request?.Reason, ct);
        var status = await _taskExecutionGateService.GetStatusAsync(ct);
        return Ok(BaseResponse<TaskExecutionStatusDto>.SuccessResult(
            status,
            "Đã tạm dừng start task mới — task đang chạy vẫn tiếp tục"));
    }

    [HttpPost("resume")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<TaskExecutionResumeResultDto>>> Resume(
        [FromBody] TaskExecutionControlRequest? request = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Nhận yêu cầu tiếp tục thực thi task. Reason={Reason}", request?.Reason);
        var result = await _taskExecutionGateService.ResumeAsync(request?.Reason, ct);
        return Ok(BaseResponse<TaskExecutionResumeResultDto>.SuccessResult(
            result,
            result.StartedCount > 0
                ? $"Đã tiếp tục và start {result.StartedCount} task đang chờ"
                : "Đã tiếp tục thực thi task — không có task nào đang chờ"));
    }
}
