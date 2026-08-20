using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Api.Services;

namespace Wcs.Api.Controllers;

/// <summary>
/// Điều khiển AMR toàn hệ thống (dừng / tiếp tục qua RCS).
/// </summary>
[ApiController]
[Route("api/amr")]
public class AmrControlController(
    AmrControlService amrControlService,
    ILogger<AmrControlController> logger) : ControllerBase
{
    private readonly AmrControlService _amrControlService = amrControlService;
    private readonly ILogger<AmrControlController> _logger = logger;

    /// <summary>
    /// Lấy trạng thái dừng/tiếp tục AMR toàn hệ thống (lưu trong Settings).
    /// </summary>
    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<AmrControlStatusDto>>> GetStatus(CancellationToken ct = default)
    {
        var status = await _amrControlService.GetStatusAsync(ct);
        return Ok(BaseResponse<AmrControlStatusDto>.SuccessResult(status, "Lấy trạng thái AMR thành công"));
    }

    /// <summary>
    /// Dừng ngay lập tức toàn bộ AMR đang active bằng RCS stopRobot.
    /// </summary>
    [HttpPost("stop-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<BaseResponse<StopAllAmrResultDto>>> StopAllImmediate(
        [FromBody] StopAllAmrRequest? request = null,
        CancellationToken ct = default)
    {
        _logger.LogWarning("Nhận yêu cầu dừng ngay toàn bộ AMR. Reason={Reason}", request?.Reason);

        var result = await _amrControlService.StopAllImmediateAsync(request?.Reason, ct);

        if (result.Results.Count == 0)
        {
            return Ok(BaseResponse<StopAllAmrResultDto>.SuccessResult(
                result,
                "Không có AMR active để dừng"));
        }

        if (result.FailedCount > 0)
        {
            var partialResponse = BaseResponse<StopAllAmrResultDto>.ErrorResult(
                $"Dừng AMR hoàn tất một phần: {result.StoppedCount} thành công, {result.FailedCount} thất bại",
                "AMR_STOP_PARTIAL_FAILURE",
                result.Results.Where(r => !r.Success).Select(r => $"{r.RobotCode}: {r.Message}").ToList());
            partialResponse.Data = result;

            return StatusCode(StatusCodes.Status502BadGateway, partialResponse);
        }

        return Ok(BaseResponse<StopAllAmrResultDto>.SuccessResult(
            result,
            $"Đã dừng ngay lập tức {result.StoppedCount} AMR"));
    }

    /// <summary>
    /// Tiếp tục toàn bộ AMR đang active bằng RCS resumeRobot.
    /// </summary>
    [HttpPost("resume-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<ActionResult<BaseResponse<ResumeAllAmrResultDto>>> ResumeAll(
        [FromBody] ResumeAllAmrRequest? request = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Nhận yêu cầu resume toàn bộ AMR. Reason={Reason}", request?.Reason);

        var result = await _amrControlService.ResumeAllAsync(request?.Reason, ct);

        if (result.Results.Count == 0)
        {
            return Ok(BaseResponse<ResumeAllAmrResultDto>.SuccessResult(
                result,
                "Không có AMR active để resume"));
        }

        if (result.FailedCount > 0)
        {
            var partialResponse = BaseResponse<ResumeAllAmrResultDto>.ErrorResult(
                $"Resume AMR hoàn tất một phần: {result.ResumedCount} thành công, {result.FailedCount} thất bại",
                "AMR_RESUME_PARTIAL_FAILURE",
                result.Results.Where(r => !r.Success).Select(r => $"{r.RobotCode}: {r.Message}").ToList());
            partialResponse.Data = result;

            return StatusCode(StatusCodes.Status502BadGateway, partialResponse);
        }

        return Ok(BaseResponse<ResumeAllAmrResultDto>.SuccessResult(
            result,
            $"Đã resume {result.ResumedCount} AMR"));
    }
}
