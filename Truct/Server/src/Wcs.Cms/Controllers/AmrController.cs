using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class AmrController(AmrService amrService, ILogger<AmrController> logger) : ControllerBase
{
    private readonly AmrService _amrService = amrService;
    private readonly ILogger<AmrController> _logger = logger;

    /// <summary>
    /// Hiển thị danh sách amr
    /// </summary>
    [HttpGet("amrs")]
    public async Task<ActionResult<BaseResponse<IEnumerable<AmrResponse>>>> List()
    {
        var amrs = await _amrService.GetListAsync();
        return Ok(BaseResponse<IEnumerable<AmrResponse>>.SuccessResult(
            [.. amrs.Select(AmrResponse.FromAmr)],
            "Lấy danh sách AMR thành công",
            new Meta { Total = amrs.Count() }
        ));
    }

    [HttpGet("amrs/area")]
    public async Task<ActionResult<BaseResponse<IEnumerable<AmrResponse>>>> ListByArea([FromQuery] AmrByAreaListRequest request)
    {
        var amrs = await _amrService.GetListByAreaAsync(request);
        return Ok(BaseResponse<IEnumerable<AmrResponse>>.SuccessResult(
            [.. amrs.Select(AmrResponse.FromAmr)],
            "Lấy danh sách Amr thành công"
        ));
    }

    /// <summary>
    /// Hiển thị amr theo code
    /// </summary>
    [HttpGet("amrs/{id}")]
    public async Task<ActionResult<BaseResponse<AmrResponse>>> Show(string id)
    {
        var amr = await _amrService.ShowAmrAsync(id);
        return Ok(BaseResponse<AmrResponse>.SuccessResult(AmrResponse.FromAmr(amr), "Lấy thông tin AMR thành công"));
    }

    /// <summary>
    /// Tạo amr mới
    /// </summary>
    [HttpPost("amr")]
    public async Task<ActionResult<BaseResponse<AmrResponse>>> Create(AmrCreateRequest request)
    {
        var amr = await _amrService.CreateAmrAsync(request);
        return Ok(BaseResponse<AmrResponse>.SuccessResult(AmrResponse.FromAmr(amr), "Tạo AMR thành công"));
    }

    /// <summary>
    /// Tạo amr mới
    /// </summary>
    [HttpPost("amrs")]
    public async Task<ActionResult<BaseResponse<AmrResponse>>> Create(List<AmrCreateRequest> requests)
    {
        var amrs = await _amrService.CreateAmrsAsync(requests);
        return Ok(BaseResponse<IEnumerable<AmrResponse>>.SuccessResult(
            [.. amrs.Select(AmrResponse.FromAmr)],
            "Đồng bộ danh sách AMR thành công",
            new Meta { Total = amrs.Count() }
        ));
    }

    /// <summary>
    /// Cập nhật amr
    /// </summary>
    [HttpPut("amrs/{id}")]
    public async Task<ActionResult<BaseResponse<AmrResponse>>> Update(string id, AmrUpdateRequest request)
    {
        var amr = await _amrService.UpdateAmrAsync(id, request);
        return Ok(BaseResponse<AmrResponse>.SuccessResult(AmrResponse.FromAmr(amr), "Cập nhật AMR thành công"));
    }

    /// <summary>
    /// Xóa amr (soft delete)
    /// </summary>
    [HttpDelete("amrs/{id}")]
    public async Task<ActionResult<BaseResponse<object>>> Delete(string id)
    {
        await _amrService.DeleteAmrAsync(id);
        return Ok(BaseResponse<object>.SuccessResult("Xóa AMR thành công"));
    }
}

