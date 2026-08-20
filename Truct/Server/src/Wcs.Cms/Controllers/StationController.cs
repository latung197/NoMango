using Microsoft.AspNetCore.Mvc;
using Wcs.Cms.Controllers.DTOs.Requests;
using Wcs.Cms.Controllers.DTOs.Responses;
using Wcs.Cms.Services;
using Wcs.Common.Entities;

namespace Wcs.Cms.Controllers;

[ApiController]
[Route("api/")]
public class StationController(StationService stationService, ILogger<StationController> logger) : ControllerBase
{
    private readonly StationService _stationService = stationService;
    private readonly ILogger<StationController> _logger = logger;

    /// <summary>
    /// Hiển thị danh sách station
    /// </summary>
    [HttpGet("stations")]
    public async Task<ActionResult<BaseResponse<IEnumerable<StationResponse>>>> List([FromQuery] StationListRequest request)
    {
        var stations = await _stationService.GetListAsync(request);
        var total = await _stationService.CountAsync(request);
        return Ok(BaseResponse<IEnumerable<StationResponse>>.SuccessResult(
            [.. stations.Select(StationResponse.FromStation)],
            "Lấy danh sách Station thành công",
            new Meta { Total = total, CurrentPage = request.PageNumber, Size = request.PageSize }
        ));
    }

    /// <summary>
    /// Hiển thị station theo id
    /// </summary>
    [HttpGet("stations/{id}")]
    public async Task<ActionResult<BaseResponse<StationResponse>>> Show(string id)
    {
        var station = await _stationService.ShowStationAsync(id);
        return Ok(BaseResponse<StationResponse>.SuccessResult(StationResponse.FromStation(station), "Lấy thông tin Station thành công"));
    }

    /// <summary>
    /// Tạo station mới
    /// </summary>
    [HttpPost("stations")]
    public async Task<ActionResult<BaseResponse<StationResponse>>> Create(StationCreateRequest request)
    {
        var station = await _stationService.CreateStationAsync(request);
        return Ok(BaseResponse<StationResponse>.SuccessResult(StationResponse.FromStation(station), "Tạo station thành công"));
    }

    /// <summary>
    /// Cập nhật station
    /// </summary>
    [HttpPut("stations/{id}")]
    public async Task<ActionResult<BaseResponse<StationResponse>>> Update(string id, StationUpdateRequest request)
    {
        var station = await _stationService.UpdateStationAsync(id, request);
        return Ok(BaseResponse<StationResponse>.SuccessResult(StationResponse.FromStation(station), "Cập nhật station thành công"));
    }

    /// <summary>
    /// Xóa station
    /// </summary>
    [HttpDelete("stations/{id}")]
    public async Task<ActionResult<BaseResponse<object>>> Delete(string id)
    {
        await _stationService.DeleteStationAsync(id);
        return Ok(BaseResponse<object>.SuccessResult("Xóa station thành công"));
    }
}

