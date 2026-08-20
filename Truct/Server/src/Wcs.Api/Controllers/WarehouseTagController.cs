using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Okamura;
using Wcs.Okamura.Enums;
using Wcs.Okamura.Services;
using Wcs.OpcUa.Contracts;

namespace Wcs.Api.Controllers;

/// <summary>
/// Controller dùng để test và kiểm tra toàn bộ tag OPC liên quan đến kho (Crane + CV/AGV)
/// </summary>
[ApiController]
[Route("api/warehouse")]
[Produces("application/json")]
public class WarehouseTagController(
    IOpcUaClient opcUaClient,
    ICraneService craneService,
    IStationHandshakeService stationHandshakeService,
    ILogger<WarehouseTagController> logger) : ControllerBase
{
    private readonly IOpcUaClient _opc = opcUaClient;
    private readonly ICraneService _crane = craneService;
    private readonly IStationHandshakeService _handshake = stationHandshakeService;
    private readonly ILogger<WarehouseTagController> _logger = logger;

    // =====================================================================
    // Định nghĩa metadata cho từng tag OPC của Crane (Stacker)
    // =====================================================================
    private static readonly List<WarehouseTagMeta> CraneTagMetas =
    [
        // PC → PLC (Instruction Data)
        new(OpcTags.D3000, "D3000", "Heartbeat WCS→PLC", "PC→PLC", "Heartbeat", null),
        new(OpcTags.D3001, "D3001", "Mã thiết bị (WCS ghi)", "PC→PLC", "Instruction", null),
        new(OpcTags.D3002, "D3002", "Loại hàng hóa (taryType)", "PC→PLC", "Instruction", null),
        new(OpcTags.D3003, "D3003", "Vị trí xuất phát – Row", "PC→PLC", "Position", null),
        new(OpcTags.D3004, "D3004", "Vị trí xuất phát – Position", "PC→PLC", "Position", null),
        new(OpcTags.D3005, "D3005", "Vị trí xuất phát – Floor", "PC→PLC", "Position", null),
        new(OpcTags.D3006, "D3006", "Vị trí đích – Row", "PC→PLC", "Position", null),
        new(OpcTags.D3007, "D3007", "Vị trí đích – Position", "PC→PLC", "Position", null),
        new(OpcTags.D3008, "D3008", "Vị trí đích – Floor", "PC→PLC", "Position", null),
        new(OpcTags.D3009, "D3009", "Loại task (1=Inbound, 2=Outbound, 3=InternalMove, 4=StationMove)", "PC→PLC", "Instruction", "CraneTaskType"),
        new(OpcTags.D3010, "D3010", "Lệnh thực thi task (1=Execute)", "PC→PLC", "Control", null),
        new(OpcTags.D3011, "D3011", "Lệnh dừng (stStop)", "PC→PLC", "Control", null),
        new(OpcTags.D3012, "D3012", "Lệnh reset (stReset)", "PC→PLC", "Control", null),
        new(OpcTags.D3013, "D3013", "Xác nhận hoàn thành task (0=None, 1=Auto, 2=Manual, 3=Cancel)", "PC→PLC", "Control", "TaskCompleteStatus"),
        new(OpcTags.D3014, "D3014", "Xác nhận lỗi task", "PC→PLC", "Control", null),
        new(OpcTags.D3015, "D3015", "Xác nhận nhận task (taskInAffirm)", "PC→PLC", "Control", null),
        new(OpcTags.D3016, "D3016", "Số task (TaskNo)", "PC→PLC", "Instruction", null),

        // PLC → PC (Confirmation/Feedback Data)
        new(OpcTags.D3040, "D3040", "Heartbeat PLC→WCS", "PLC→PC", "Heartbeat", null),
        new(OpcTags.D3041, "D3041", "Mã thiết bị (PLC phản hồi)", "PLC→PC", "Info", null),
        new(OpcTags.D3042, "D3042", "Thông tin đường dẫn (tunnel)", "PLC→PC", "Info", null),
        new(OpcTags.D3043, "D3043", "Vị trí xuất phát – Row (PLC)", "PLC→PC", "Position", null),
        new(OpcTags.D3044, "D3044", "Vị trí xuất phát – Position (PLC)", "PLC→PC", "Position", null),
        new(OpcTags.D3045, "D3045", "Vị trí xuất phát – Floor (PLC)", "PLC→PC", "Position", null),
        new(OpcTags.D3046, "D3046", "Vị trí đích – Row (PLC)", "PLC→PC", "Position", null),
        new(OpcTags.D3047, "D3047", "Vị trí đích – Position (PLC)", "PLC→PC", "Position", null),
        new(OpcTags.D3048, "D3048", "Vị trí đích – Floor (PLC)", "PLC→PC", "Position", null),
        new(OpcTags.D3049, "D3049", "Loại task hiện tại (PLC)", "PLC→PC", "Feedback", "CraneTaskType"),
        new(OpcTags.D3050, "D3050", "Phản hồi thực thi (0=None, 1=Accepted, 2=InstructionError)", "PLC→PC", "Feedback", "TaskExecuteFeedback"),
        new(OpcTags.D3051, "D3051", "Mã lỗi (Error Code)", "PLC→PC", "Error", null),
        new(OpcTags.D3052, "D3052", "Ghi chú lỗi (Error Note / taryType feedback)", "PLC→PC", "Error", null),
        new(OpcTags.D3053, "D3053", "Trạng thái hoàn thành (0=None, 1=Auto, 2=Manual, 3=Canceled)", "PLC→PC", "Feedback", "TaskCompleteStatus"),
        new(OpcTags.D3054, "D3054", "Lỗi đặc biệt (0=None, 1=NoItemAtPick, 2=PutOccupied, 3=BlockedPick, 4=BlockedPut)", "PLC→PC", "Error", "CraneSpecialErrorType"),
        new(OpcTags.D3055, "D3055", "Tín hiệu nhận hàng (claimGoods)", "PLC→PC", "Feedback", null),
        new(OpcTags.D3056, "D3056", "Số task hiện tại (PLC)", "PLC→PC", "Feedback", null),
        new(OpcTags.D3057, "D3057", "Trạng thái sẵn sàng nhận lệnh (dispatch/prepareCompleted)", "PLC→PC", "Feedback", null),
        new(OpcTags.D3059, "D3059", "Trạng thái vận hành crane (1=Auto, 2=Manual, 3=Online, 4=Maintenance)", "PLC→PC", "Status", "CraneStatus"),

        // Position / Encoder
        new(OpcTags.D3061, "D3061", "Vị trí thực tế X (posX)", "PLC→PC", "Position", null),
        new(OpcTags.D3062, "D3062", "Vị trí thực tế Y (posY)", "PLC→PC", "Position", null),
        new(OpcTags.D3063, "D3063", "Vị trí thực tế Z (posZ)", "PLC→PC", "Position", null),
        new(OpcTags.D3064, "D3064", "Encoder vị trí X (locatX)", "PLC→PC", "Position", null),
        new(OpcTags.D3066, "D3066", "Encoder vị trí Y (locatY)", "PLC→PC", "Position", null),
        new(OpcTags.D3068, "D3068", "Encoder vị trí Z / Fork (locatZ)", "PLC→PC", "Position", null),
    ];

    // =====================================================================
    // Định nghĩa metadata cho tag CV/AGV Handshake
    // =====================================================================
    private static readonly List<WarehouseTagMeta> StationTagMetas =
    [
        // INBOUND – CV → AGV
        new(OpcTagsCvAgv.D2001, "D2001", "[IN][CV→AGV] Sẵn sàng hoạt động / Ready", "CV→AGV", "Inbound", null),
        new(OpcTagsCvAgv.D2002, "D2002", "[IN][CV→AGV] Cho phép bàn giao hàng", "CV→AGV", "Inbound", null),
        new(OpcTagsCvAgv.D2003, "D2003", "[IN][CV→AGV] Cảm biến an toàn tắt/đóng", "CV→AGV", "Inbound", null),
        new(OpcTagsCvAgv.D2004, "D2004", "[IN][CV→AGV] Băng tải/bàn quay đang chạy", "CV→AGV", "Inbound", null),
        new(OpcTagsCvAgv.D2021, "D2021", "[IN][CV→AGV] Cassette sẵn sàng cho crane lấy (CraneReady)", "CV→AGV", "Inbound", null),
        new(OpcTagsCvAgv.D2024, "D2024", "[IN][CV→AGV] Trạng thái rèm an toàn (0=bật, 1=mute/thủ công)", "CV→AGV", "Inbound", "SafetyCurtainStatus"),
        new(OpcTagsCvAgv.D2028, "D2028", "[IN][CV→AGV] Cảm biến có hàng tại cửa kho nhập (1=có, 0=không)", "CV→AGV", "Inbound", null),
        new(OpcTagsCvAgv.D2030, "D2030", "[IN][CV→AGV] Cảm biến có hàng tại cửa kho nhập #2 (1=có, 0=không)", "CV→AGV", "Inbound", null),
        new(OpcTagsCvAgv.D2351_2360, "D2351–D2360", "[IN][CV→AGV] QR block (D2351–D2360, 10 words)", "CV→AGV", "Inbound/QR", null),
        new(OpcTagsCvAgv.D2361,     "D2361",       "[IN][CV→AGV] QR Ready flag", "CV→AGV", "Inbound/QR", null),

        // INBOUND – AGV → CV
        new(OpcTagsCvAgv.D2101, "D2101", "[IN][AGV→CV] Sẵn sàng hoạt động / Ready", "AGV→CV", "Inbound", null),
        new(OpcTagsCvAgv.D2102, "D2102", "[IN][AGV→CV] AGV đã đến trạm", "AGV→CV", "Inbound", null),
        new(OpcTagsCvAgv.D2103, "D2103", "[IN][AGV→CV] Đang bàn giao hàng", "AGV→CV", "Inbound", null),
        new(OpcTagsCvAgv.D2104, "D2104", "[IN][AGV→CV] Hành vi xâm nhập của AGV", "AGV→CV", "Inbound", null),
        new(OpcTagsCvAgv.D2451, "D2451", "[IN][AGV→CV] QR Finished flag", "AGV→CV", "Inbound/QR", null),

        // OUTBOUND – CV → AGV
        new(OpcTagsCvAgv.D2011, "D2011", "[OUT][CV→AGV] Sẵn sàng hoạt động / Ready", "CV→AGV", "Outbound", null),
        new(OpcTagsCvAgv.D2012, "D2012", "[OUT][CV→AGV] Yêu cầu AGV đến trạm", "CV→AGV", "Outbound", null),
        new(OpcTagsCvAgv.D2013, "D2013", "[OUT][CV→AGV] Cảm biến an toàn đóng/tắt", "CV→AGV", "Outbound", null),
        new(OpcTagsCvAgv.D2014, "D2014", "[OUT][CV→AGV] Băng tải/bàn quay đang chạy", "CV→AGV", "Outbound", null),
        new(OpcTagsCvAgv.D2022, "D2022", "[OUT][CV→AGV] Cassette sẵn sàng cho AGV lấy (CraneReady)", "CV→AGV", "Outbound", null),
        new(OpcTagsCvAgv.D2026, "D2026", "[OUT][CV→AGV] Trạng thái rèm an toàn (0=bật, 1=mute/thủ công)", "CV→AGV", "Outbound", "SafetyCurtainStatus"),
        new(OpcTagsCvAgv.D2027, "D2027", "[OUT][CV→AGV] Cảm biến vật lý có hàng tại cửa kho xuất (1=có, 0=không)", "CV→AGV", "Outbound", null),
        new(OpcTagsCvAgv.D2029, "D2029", "[OUT][CV→AGV] Cảm biến có hàng tại cửa kho xuất #2 (1=có, 0=không)", "CV→AGV", "Outbound", null),
        new(OpcTagsCvAgv.D2311_2320, "D2311–D2320", "[OUT][CV→AGV] QR block (D2311–D2320, 10 words)", "CV→AGV", "Outbound/QR", null),
        new(OpcTagsCvAgv.D2321,     "D2321",       "[OUT][CV→AGV] QR Ready flag (PLC quét xong, WCS đọc được)", "CV→AGV", "Outbound/QR", null),

        // OUTBOUND – AGV → CV
        new(OpcTagsCvAgv.D2111, "D2111", "[OUT][AGV→CV] Sẵn sàng hoạt động / Ready", "AGV→CV", "Outbound", null),
        new(OpcTagsCvAgv.D2112, "D2112", "[OUT][AGV→CV] AGV đã đến trạm", "AGV→CV", "Outbound", null),
        new(OpcTagsCvAgv.D2113, "D2113", "[OUT][AGV→CV] AGV đang di chuyển / chuyển hàng", "AGV→CV", "Outbound", null),
        new(OpcTagsCvAgv.D2114, "D2114", "[OUT][AGV→CV] Hành vi xâm nhập của AGV", "AGV→CV", "Outbound", null),
        new(OpcTagsCvAgv.D2115, "D2115", "[OUT][AGV→CV] Hành động tiếp nhận hàng của AGV", "AGV→CV", "Outbound", null),
        new(OpcTagsCvAgv.D2411, "D2411", "[OUT][AGV→CV] QR Finished flag", "AGV→CV", "Outbound/QR", null),
    ];

    /// <summary>
    /// Đọc toàn bộ tag OPC của crane (Stacker) và trả về cùng metadata + giá trị hiện tại
    /// </summary>
    [HttpGet("tags/crane")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCraneTags()
    {
        var results = await ReadTagsAsync(CraneTagMetas);
        return Ok(new WarehouseTagsResponse
        {
            Group = "Crane (Stacker)",
            TotalTags = results.Count,
            ConnectedCount = results.Count(t => t.IsGood),
            Timestamp = DateTime.Now,
            Tags = results
        });
    }

    /// <summary>
    /// Đọc toàn bộ tag OPC của CV/AGV handshake và trả về cùng metadata + giá trị hiện tại
    /// </summary>
    [HttpGet("tags/station")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStationTags()
    {
        var results = await ReadTagsAsync(StationTagMetas);
        return Ok(new WarehouseTagsResponse
        {
            Group = "Station CV/AGV Handshake",
            TotalTags = results.Count,
            ConnectedCount = results.Count(t => t.IsGood),
            Timestamp = DateTime.Now,
            Tags = results
        });
    }

    /// <summary>
    /// Đọc toàn bộ tất cả tag OPC (Crane + Station) trong một lần gọi
    /// </summary>
    [HttpGet("tags")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTags()
    {
        var craneTags = await ReadTagsAsync(CraneTagMetas);
        var stationTags = await ReadTagsAsync(StationTagMetas);

        return Ok(new
        {
            timestamp = DateTime.Now,
            isConnected = craneTags.Any(t => t.IsGood) || stationTags.Any(t => t.IsGood),
            crane = new WarehouseTagsResponse
            {
                Group = "Crane (Stacker)",
                TotalTags = craneTags.Count,
                ConnectedCount = craneTags.Count(t => t.IsGood),
                Timestamp = DateTime.Now,
                Tags = craneTags
            },
            station = new WarehouseTagsResponse
            {
                Group = "Station CV/AGV Handshake",
                TotalTags = stationTags.Count,
                ConnectedCount = stationTags.Count(t => t.IsGood),
                Timestamp = DateTime.Now,
                Tags = stationTags
            }
        });
    }

    /// <summary>
    /// Đọc trạng thái và phản hồi hiện tại của crane
    /// </summary>
    [HttpGet("crane/feedback")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCraneFeedback()
    {
        try
        {
            var feedback = await _crane.GetCurrentFeedbackAsync();
            return Ok(new
            {
                success = true,
                timestamp = DateTime.Now,
                data = new
                {
                    taskNo = feedback.TaskNo,
                    taskType = feedback.TaskType.ToString(),
                    taskTypeCode = (int)feedback.TaskType,
                    executeFeedback = feedback.ExecuteFeedback.ToString(),
                    executeFeedbackCode = (int)feedback.ExecuteFeedback,
                    completeStatus = feedback.CompleteStatus.ToString(),
                    completeStatusCode = (int)feedback.CompleteStatus,
                    specialError = feedback.SpecialError.ToString(),
                    specialErrorCode = (int)feedback.SpecialError,
                    status = feedback.Status.ToString(),
                    statusCode = (int)feedback.Status,
                    errorCode = feedback.ErrorCode,
                    errorNote = feedback.ErrorNote,
                    position = new
                    {
                        row = feedback.Position.Row,
                        floor = feedback.Position.Floor,
                        forkPosition = feedback.Position.Position
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không đọc được feedback crane (có thể chưa kết nối OPC)");
            return Ok(new { success = false, timestamp = DateTime.Now, error = ex.Message, data = (object?)null });
        }
    }

    /// <summary>
    /// Reset tín hiệu handshake băng tải kho (inbound + outbound):
    /// BackToWaiting → AfterDrop/AfterPickup, và đưa D2411/D2451 về 0.
    /// </summary>
    [HttpPost("station/reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetStationConveyorSignals()
    {
        var ct = HttpContext.RequestAborted;
        try
        {
            _logger.LogInformation("Reset tín hiệu băng tải kho – BackToWaiting, AfterDrop/AfterPickup, D2411/D2451=0");
            await _handshake.ResetWarehouseConveyorSignalsAsync(ct);

            return Ok(new
            {
                success = true,
                timestamp = DateTime.Now,
                stepsCompleted = new[]
                {
                    "Inbound_BackToDropWaitingPoint",
                    "Inbound_AfterDrop",
                    "Outbound_BackToPickupWaitingPoint",
                    "Outbound_AfterPickup",
                    "D2411=0",
                    "D2451=0",
                },
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi reset tín hiệu băng tải kho");
            return Ok(new { success = false, timestamp = DateTime.Now, error = ex.Message });
        }
    }

    /// <summary>
    /// Trả về danh sách tất cả tag definition (không đọc giá trị) – hữu ích để xem metadata
    /// </summary>
    [HttpGet("tags/definitions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetTagDefinitions()
    {
        return Ok(new
        {
            timestamp = DateTime.Now,
            crane = CraneTagMetas.Select(m => new
            {
                tagId = m.TagId,
                nodeId = m.NodeId,
                description = m.Description,
                direction = m.Direction,
                category = m.Category,
                enumType = m.EnumType
            }),
            station = StationTagMetas.Select(m => new
            {
                tagId = m.TagId,
                nodeId = m.NodeId,
                description = m.Description,
                direction = m.Direction,
                category = m.Category,
                enumType = m.EnumType
            })
        });
    }

    // ------------------------------------------------------------------
    private async Task<List<WarehouseTagValue>> ReadTagsAsync(List<WarehouseTagMeta> metas)
    {
        var results = new List<WarehouseTagValue>();
        foreach (var meta in metas)
        {
            try
            {
                var dataValue = await _opc.ReadValueAsync(meta.NodeId);
                var rawValue = dataValue.Value;
                var isGood = Opc.Ua.StatusCode.IsGood(dataValue.StatusCode);
                string? decoded = DecodeEnumValue(meta.EnumType, rawValue);

                results.Add(new WarehouseTagValue
                {
                    TagId = meta.TagId,
                    NodeId = meta.NodeId,
                    Description = meta.Description,
                    Direction = meta.Direction,
                    Category = meta.Category,
                    EnumType = meta.EnumType,
                    RawValue = rawValue,
                    DecodedValue = decoded,
                    Timestamp = dataValue.SourceTimestamp == DateTime.MinValue ? DateTime.Now : dataValue.SourceTimestamp,
                    IsGood = isGood,
                    StatusCode = dataValue.StatusCode.ToString(),
                    Error = null
                });
            }
            catch (Exception ex)
            {
                results.Add(new WarehouseTagValue
                {
                    TagId = meta.TagId,
                    NodeId = meta.NodeId,
                    Description = meta.Description,
                    Direction = meta.Direction,
                    Category = meta.Category,
                    EnumType = meta.EnumType,
                    RawValue = null,
                    DecodedValue = null,
                    Timestamp = DateTime.Now,
                    IsGood = false,
                    StatusCode = "Error",
                    Error = ex.Message
                });
            }
        }
        return results;
    }

    private static string? DecodeEnumValue(string? enumType, object? rawValue)
    {
        if (enumType == null || rawValue == null) return null;
        try
        {
            int intVal = Convert.ToInt32(rawValue);
            return enumType switch
            {
                "CraneTaskType" => ((CraneTaskType)intVal).ToString(),
                "TaskExecuteFeedback" => ((TaskExecuteFeedback)intVal).ToString(),
                "TaskCompleteStatus" => ((TaskCompleteStatus)intVal).ToString(),
                "CraneSpecialErrorType" => ((CraneSpecialErrorType)intVal).ToString(),
                "CraneStatus" => ((CraneStatus)intVal).ToString(),
                "SafetyCurtainStatus" => intVal switch
                {
                    0 => "Rèm bật",
                    1 => "Rèm tắt (thủ công)",
                    _ => intVal.ToString(),
                },
                _ => null
            };
        }
        catch
        {
            return null;
        }
    }
}

// =====================================================================
// DTOs nội bộ cho Warehouse Tag
// =====================================================================

public record WarehouseTagMeta(
    string NodeId,
    string TagId,
    string Description,
    string Direction,
    string Category,
    string? EnumType);

public class WarehouseTagsResponse
{
    public string Group { get; set; } = string.Empty;
    public int TotalTags { get; set; }
    public int ConnectedCount { get; set; }
    public DateTime Timestamp { get; set; }
    public List<WarehouseTagValue> Tags { get; set; } = [];
}

public class WarehouseTagValue
{
    public string TagId { get; set; } = string.Empty;
    public string NodeId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? EnumType { get; set; }
    public object? RawValue { get; set; }
    public string? DecodedValue { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsGood { get; set; }
    public string? StatusCode { get; set; }
    public string? Error { get; set; }
}
