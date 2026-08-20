using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Api.Services;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.Common.Extensions;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Controllers;

[ApiController]
[Route("api/stations")]
public class StationController(
    ILogger<StationController> logger,
    StationService stationService,
    StationSnapshotService stationSnapshotService) : ControllerBase
{
    private readonly ILogger<StationController> _logger = logger;
    private readonly StationService _stationService = stationService;
    private readonly StationSnapshotService _stationSnapshotService = stationSnapshotService;

    /// <summary>
    /// Lấy station theo code
    /// </summary>
    [HttpGet("by-code/{code}")]
    public async Task<ActionResult<BaseResponse<Station>>> GetStationByCode(string code)
    {
        var station = await _stationService.GetStationByCode(code);
        if (station == null) {
            return NotFound(BaseResponse<Station>.ErrorResult($"Station với Code {code} không tồn tại", "STATION_NOT_FOUND"));
        }
        return Ok(BaseResponse<Station>.SuccessResult(station, "Lấy thông tin Station thành công"));
    }

    /// <summary>
    /// Lấy requirements của station theo code và step
    /// </summary>
    [HttpGet("requirements-by-code/{code}/step/{step}")]
    public async Task<ActionResult<BaseResponse<WaitingSet>>> GetRequirementsByCode(string code, FlowStep step)
    {
        var station = await _stationService.GetStationByCode(code);
        if (station == null) {
            return NotFound(BaseResponse<Station>.ErrorResult($"Station với Code {code} không tồn tại", "STATION_NOT_FOUND"));
        }
        var requirements = await _stationService.GetWaitingRequirements(step, station);
        return Ok(BaseResponse<WaitingSet>.SuccessResult(requirements, "Lấy thông tin Requirements thành công"));
    }

    /// <summary>
    /// Lấy danh sách trạm fallback của một trạm (cùng type/size, cùng stage hoặc trạm nhập kho).
    /// </summary>
    [HttpGet("{code}/fallback-stations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<IEnumerable<Station>>>> GetFallbackStations(string code, [FromQuery(Name = "size")] int? size)
    {
        var station = await _stationService.GetStationByCode(code);
        if (station == null)
        {
            return NotFound(BaseResponse<IEnumerable<Station>>.ErrorResult(
                $"Station với Code {code} không tồn tại", "STATION_NOT_FOUND"));
        }

        Size taskSize;
        try
        {
            taskSize = StationSizeResolver.ResolveForStation(station, size);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<IEnumerable<Station>>.ErrorResult(ex.Message, "SIZE_REQUIRED"));
        }

        var fallbackStations = await _stationService.GetFallbackStationsAsync(station, taskSize);
        return Ok(BaseResponse<IEnumerable<Station>>.SuccessResult(
            fallbackStations, $"Lấy danh sách fallback station của {code} thành công"));
    }

    /// <summary>
    /// Lấy snapshot realtime của tất cả trạm theo stage
    /// </summary>
    [HttpGet("snapshots")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<StationSnapshot>>>> GetSnapshots([FromQuery] string stageCode)
    {
        if (string.IsNullOrWhiteSpace(stageCode))
        {
            return BadRequest(BaseResponse<IReadOnlyList<StationSnapshot>>.ErrorResult(
                "stageCode là bắt buộc", "STAGE_CODE_REQUIRED"));
        }

        var snapshots = await _stationSnapshotService.BuildByStageAsync(stageCode);
        return Ok(BaseResponse<IReadOnlyList<StationSnapshot>>.SuccessResult(
            snapshots, $"Lấy snapshot trạm stage {stageCode} thành công"));
    }

    /// <summary>
    /// Lấy snapshot realtime của một trạm theo code
    /// </summary>
    [HttpGet("{code}/snapshot")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<StationSnapshot>>> GetSnapshot(string code)
    {
        var snapshot = await _stationSnapshotService.BuildAsync(code);
        if (snapshot == null)
        {
            return NotFound(BaseResponse<StationSnapshot>.ErrorResult(
                $"Station với Code {code} không tồn tại", "STATION_NOT_FOUND"));
        }

        return Ok(BaseResponse<StationSnapshot>.SuccessResult(
            snapshot, $"Lấy snapshot station {code} thành công"));
    }

    /// <summary>
    /// Đọc toàn bộ giá trị các tag OPC UA của một trạm theo StationCode
    /// </summary>
    /// <param name="code">Mã trạm (StationCode)</param>
    /// <returns>Danh sách tên tag và giá trị hiện tại đọc từ OPC UA server</returns>
    /// <response code="200">Đọc tag thành công</response>
    /// <response code="404">Không tìm thấy station</response>
    /// <response code="500">Lỗi nội bộ server</response>
    [HttpGet("{code}/opc-tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<StationOpcTagsResponse>>> GetOpcTags(string code)
    {
        try
        {
            var station = await _stationService.GetStationByCode(code);
            if (station == null)
                return NotFound(BaseResponse<StationOpcTagsResponse>.ErrorResult(
                    $"Station với Code {code} không tồn tại", "STATION_NOT_FOUND"));

            var result = await _stationService.GetStationOpcTagsAsync(station);
            return Ok(BaseResponse<StationOpcTagsResponse>.SuccessResult(
                result, $"Đọc tag OPC của station {code} thành công"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đọc tag OPC cho station {Code}", code);
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<StationOpcTagsResponse>.ErrorResult("Lỗi nội bộ server", "INTERNAL_ERROR"));
        }
    }
}

