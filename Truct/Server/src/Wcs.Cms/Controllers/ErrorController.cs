using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class ErrorController(ErrorService errorService, ILogger<ErrorController> logger) : ControllerBase
{
    private readonly ErrorService _errorService = errorService;
    private readonly ILogger<ErrorController> _logger = logger;

    /// <summary>
    /// Hiển thị danh sách error
    /// </summary>
    [HttpGet("errors")]
    public async Task<ActionResult<BaseResponse<IEnumerable<ErrorResponse>>>> List([FromQuery] ErrorListRequest request)
    {
        var errors = await _errorService.GetListAsync(request);
        return Ok(BaseResponse<IEnumerable<ErrorResponse>>.SuccessResult(
            [.. errors.Select(ErrorResponse.FromError)],
            "Lấy danh sách Cảnh báo thành công",
            new Meta { Total = errors.Count() }
            //new Meta { Total = errors.Count(), CurrentPage = request.PageNumber, Size = request.PageSize }
        ));
    }

    /*[HttpGet("errors/area")]
    public async Task<ActionResult<BaseResponse<IEnumerable<ErrorResponse>>>> ListByArea([FromQuery] ErrorByAreaListRequest request)
    {
        var errors = await _errorService.GetListByAreaAsync(request);
        return Ok(BaseResponse<IEnumerable<ErrorResponse>>.SuccessResult(
            [.. errors.Select(ErrorResponse.FromError)],
            "Lấy danh sách Error thành công"
        ));
    }*/

    /// <summary>
    /// Hiển thị error theo code
    /// </summary>
    [HttpGet("errors/{id}")]
    public async Task<ActionResult<BaseResponse<ErrorResponse>>> Show(string id)
    {
        var error = await _errorService.ShowErrorAsync(id);
        return Ok(BaseResponse<ErrorResponse>.SuccessResult(ErrorResponse.FromError(error), "Lấy thông tin Cảnh báo thành công"));
    }

    /// <summary>
    /// Tạo error mới
    /// </summary>
    [HttpPost("errors")]
    public async Task<ActionResult<BaseResponse<ErrorResponse>>> Create(ErrorCreateRequest request)
    {
        var error = await _errorService.CreateErrorAsync(request);
        return Ok(BaseResponse<ErrorResponse>.SuccessResult(ErrorResponse.FromError(error), "Tạo Cảnh báo thành công"));
    }

    /// <summary>
    /// Cập nhật error
    /// </summary>
    [HttpPut("errors/{id}")]
    public async Task<ActionResult<BaseResponse<ErrorResponse>>> Update(string id, ErrorUpdateRequest request)
    {
        var error = await _errorService.UpdateErrorAsync(id, request);
        return Ok(BaseResponse<ErrorResponse>.SuccessResult(ErrorResponse.FromError(error), "Cập nhật Cảnh báo thành công"));
    }

    /// <summary>
    /// Cập nhật errors
    /// </summary>
    [HttpPut("errors")]
    public async Task<ActionResult<IEnumerable<ErrorResponse>>> Updates(List<ErrorUpdateRequest> requests)
    {
        var errors = await _errorService.UpdatesErrorAsync(requests);
        //return Ok(BaseResponse<ErrorResponse>.SuccessResult(ErrorResponse.FromError(error), "Cập nhật Cảnh báo thành công"));
        return Ok(BaseResponse<IEnumerable<ErrorResponse>>.SuccessResult(
            [.. errors.Select(ErrorResponse.FromError)],
            "Cập nhật danh sách Cảnh báo thành công"
        ));
    }

    /// <summary>
    /// Xóa error (soft delete)
    /// </summary>
    [HttpDelete("errors/{id}")]
    public async Task<ActionResult<BaseResponse<object>>> Delete(string id)
    {
        await _errorService.DeleteErrorAsync(id);
        return Ok(BaseResponse<object>.SuccessResult("Xóa Cảnh báo thành công"));
    }
}

