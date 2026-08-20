using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Okamura;
using Wcs.Okamura.Services;
using Wcs.OpcUa.Contracts;

namespace Wcs.Api.Controllers;

/// <summary>
/// Controller test từng bước handshake CV ↔ AGV (rèm, băng tải, bàn giao hàng)
/// </summary>
[ApiController]
[Route("api/station-handshake")]
[Produces("application/json")]
public class StationHandshakeController(
    IStationHandshakeService handshake,
    IOpcUaClient opc,
    ILogger<StationHandshakeController> logger) : ControllerBase
{
    private readonly IStationHandshakeService _hs = handshake;
    private readonly IOpcUaClient _opc = opc;
    private readonly ILogger<StationHandshakeController> _logger = logger;

    // ==================================================================
    //  INBOUND (Nhập kho) – DROP side
    //  WCS ghi vùng AGV→CV: D2101-D2104 / D2451
    //  WCS đọc vùng CV→AGV: D2001-D2004 / D2361 / D2351-D2360
    // ==================================================================

    /// <summary>
    /// [Inbound – Step 1] AGV báo Ready (D2102=1), chờ CV Ready+AllowHandover+Safety và D2028=0 (không có hàng)
    /// </summary>
    [HttpPost("inbound/before-drop")]
    public async Task<IActionResult> Inbound_BeforeDrop(
        [FromQuery] int timeoutSeconds = 30)
    {
        return await RunStep("Inbound_BeforeDrop", async ct =>
        {
            await _hs.Inbound_BeforeDropAsync(TimeSpan.FromSeconds(timeoutSeconds), ct);
        });
    }

    /// <summary>
    /// [Inbound – Step 2] AGV tới điểm thả hàng (D2103=1, D2104=1)
    /// </summary>
    [HttpPost("inbound/enter-drop")]
    public async Task<IActionResult> Inbound_EnterDrop()
    {
        return await RunStep("Inbound_EnterDropPoint", ct => _hs.Inbound_EnterDropPointAsync(ct));
    }

    /// <summary>
    /// [Inbound – Step 3] Đặt hàng xuống (no-op hiện tại – hook point)
    /// </summary>
    [HttpPost("inbound/putdown-rack")]
    public async Task<IActionResult> Inbound_PutdownRack()
    {
        return await RunStep("Inbound_PutdownRack", ct => _hs.Inbound_PutdownRackAsync(ct));
    }

    /// <summary>
    /// [Inbound – Step 4] Quay về điểm chờ (D2103=0, D2104=0)
    /// </summary>
    [HttpPost("inbound/back-to-waiting")]
    public async Task<IActionResult> Inbound_BackToWaiting()
    {
        return await RunStep("Inbound_BackToDropWaitingPoint", ct => _hs.Inbound_BackToDropWaitingPointAsync(ct));
    }

    /// <summary>
    /// [Inbound – Step 5] Kết thúc nhập kho (D2102=0). QR và CraneReady xử lý riêng.
    /// </summary>
    [HttpPost("inbound/after-drop")]
    public async Task<IActionResult> Inbound_AfterDrop(
        [FromQuery] bool waitCraneReady = false)
    {
        return await RunStep("Inbound_AfterDrop", ct =>
            _hs.Inbound_AfterDropAsync(ct, waitCraneReady));
    }

    /// <summary>
    /// [Inbound] Chờ D2361=1 – CV đã đọc QR sau khi AGV thả hàng
    /// </summary>
    [HttpPost("inbound/wait-qr-ready")]
    public async Task<IActionResult> Inbound_WaitQrReady([FromQuery] int timeoutSeconds = 120)
    {
        return await RunStep("Inbound_WaitQrReady", async ct =>
        {
            await _hs.WaitInboundQrReadyAsync(TimeSpan.FromSeconds(timeoutSeconds), ct);
        });
    }

    /// <summary>
    /// [Inbound] Đọc QR code từ CV (chỉ khi D2361=1). Có thể truyền expectedQr để kiểm tra khớp.
    /// </summary>
    [HttpGet("inbound/read-qr")]
    public async Task<IActionResult> Inbound_ReadQr([FromQuery] string? expectedQr = null)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var qr = await _hs.Inbound_ReadQrCodeAsync(cts.Token);
            var mismatch = !string.IsNullOrEmpty(expectedQr)
                && !string.Equals(qr, expectedQr, StringComparison.Ordinal);
            if (mismatch)
                _logger.LogWarning("QR mismatch – expected: {Expected}, actual: {Actual}", expectedQr, qr);

            return Ok(new { success = true, qrCode = qr, hasQr = qr != null, mismatch, timestamp = DateTime.Now });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lỗi đọc QR Inbound");
            return Ok(new { success = false, qrCode = (string?)null, error = ex.Message, timestamp = DateTime.Now });
        }
    }

    // ==================================================================
    //  OUTBOUND (Xuất kho) – PICK side
    //  WCS ghi vùng AGV→CV: D2111-D2115 / D2411
    //  WCS đọc vùng CV→AGV: D2011-D2014 / D2027 / D2321 / D2311-D2320
    // ==================================================================

    /// <summary>
    /// [Outbound – Step 1] AGV báo Ready (D2112=1), chờ CV Ready+RequestAgv+Safety
    /// </summary>
    [HttpPost("outbound/before-pick")]
    public async Task<IActionResult> Outbound_BeforePick([FromQuery] int timeoutSeconds = 30)
    {
        return await RunStep("Outbound_BeforePick", async ct =>
        {
            await _hs.Outbound_BeforePickAsync(TimeSpan.FromSeconds(timeoutSeconds), ct);
        });
    }

    /// <summary>
    /// [Outbound – Step 2] AGV tới điểm lấy hàng (D2113=1, D2114=1)
    /// </summary>
    [HttpPost("outbound/enter-pickup")]
    public async Task<IActionResult> Outbound_EnterPickup()
    {
        return await RunStep("Outbound_EnterPickupPoint", ct => _hs.Outbound_EnterPickupPointAsync(ct));
    }

    /// <summary>
    /// [Outbound – Step 3] Nâng hàng lên (no-op hiện tại – hook point)
    /// </summary>
    [HttpPost("outbound/lift-rack")]
    public async Task<IActionResult> Outbound_LiftRack()
    {
        return await RunStep("Outbound_LiftRack", ct => _hs.Outbound_LiftRackAsync(ct));
    }

    /// <summary>
    /// [Outbound – Step 4] Quay về điểm chờ (D2113=0, D2114=0)
    /// </summary>
    [HttpPost("outbound/back-to-waiting")]
    public async Task<IActionResult> Outbound_BackToWaiting()
    {
        return await RunStep("Outbound_BackToPickupWaitingPoint", ct => _hs.Outbound_BackToPickupWaitingPointAsync(ct));
    }

    /// <summary>
    /// [Outbound – Step 5] Kết thúc xuất kho (D2112=0)
    /// </summary>
    [HttpPost("outbound/after-pickup")]
    public async Task<IActionResult> Outbound_AfterPickup()
    {
        return await RunStep("Outbound_AfterPickup", ct => _hs.Outbound_AfterPickupAsync(ct));
    }

    /// <summary>
    /// [Outbound] Đọc QR code từ CV (chỉ khi D2321=1)
    /// </summary>
    [HttpGet("outbound/read-qr")]
    public async Task<IActionResult> Outbound_ReadQr()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var qr = await _hs.Outbound_ReadQrCodeAsync(cts.Token);
            return Ok(new { success = true, qrCode = qr, hasQr = qr != null, timestamp = DateTime.Now });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lỗi đọc QR Outbound");
            return Ok(new { success = false, qrCode = (string?)null, error = ex.Message, timestamp = DateTime.Now });
        }
    }

    // ==================================================================
    //  Đọc / Ghi tag trực tiếp (manual debug)
    // ==================================================================

    /// <summary>
    /// Đọc snapshot tất cả tag station handshake (cả inbound lẫn outbound) kèm phân tích điều kiện
    /// </summary>
    [HttpGet("tags/snapshot")]
    public async Task<IActionResult> GetTagsSnapshot()
    {
        var tags = new Dictionary<string, (string NodeId, string Label)>
        {
            // INBOUND CV→AGV (read)
            ["in_cv_ready"]       = (OpcTagsCvAgv.D2001, "CV_In Ready (D2001)"),
            ["in_cv_allow"]       = (OpcTagsCvAgv.D2002, "CV_In AllowHandover (D2002)"),
            ["in_cv_safety"]      = (OpcTagsCvAgv.D2003, "CV_In Safety OK (D2003)"),
            ["in_cv_conveyor"]    = (OpcTagsCvAgv.D2004, "CV_In Conveyor Running (D2004)"),
            ["in_cv_crane_ready"] = (OpcTagsCvAgv.D2021, "CV_In CraneReady (D2021)"),
            ["in_cv_curtain"]     = (OpcTagsCvAgv.D2024, "CV_In SafetyCurtain (D2024)"),
            ["in_cv_has_box"]     = (OpcTagsCvAgv.D2028, "CV_In HasBox (D2028)"),
            ["in_cv_has_box2"]    = (OpcTagsCvAgv.D2030, "CV_In HasBox2 (D2030)"),
            // INBOUND AGV→CV (write)
            ["in_agv_ready"]      = (OpcTagsCvAgv.D2101, "AGV_In Ready (D2101)"),
            ["in_agv_arrived"]    = (OpcTagsCvAgv.D2102, "AGV_In Arrived (D2102)"),
            ["in_agv_inprogress"] = (OpcTagsCvAgv.D2103, "AGV_In HandoverInProgress (D2103)"),
            ["in_agv_intrusion"]  = (OpcTagsCvAgv.D2104, "AGV_In Intrusion (D2104)"),
            ["in_agv_qr_done"]    = (OpcTagsCvAgv.D2451, "AGV_In QR Finished (D2451)"),
            ["in_cv_qr_ready"]    = (OpcTagsCvAgv.D2361, "CV_In QR Ready (D2361)"),

            // OUTBOUND CV→AGV (read)
            ["out_cv_ready"]      = (OpcTagsCvAgv.D2011, "CV_Out Ready (D2011)"),
            ["out_cv_request"]    = (OpcTagsCvAgv.D2012, "CV_Out RequestAgvArrive (D2012)"),
            ["out_cv_safety"]     = (OpcTagsCvAgv.D2013, "CV_Out Safety OK (D2013)"),
            ["out_cv_conveyor"]   = (OpcTagsCvAgv.D2014, "CV_Out Conveyor Running (D2014)"),
            ["out_cv_crane_ready"]= (OpcTagsCvAgv.D2022, "CV_Out CraneReady (D2022)"),
            ["out_cv_has_box"]    = (OpcTagsCvAgv.D2027, "CV_Out HasBox (D2027)"),
            ["out_cv_has_box2"]   = (OpcTagsCvAgv.D2029, "CV_Out HasBox2 (D2029)"),
            ["out_cv_curtain"]    = (OpcTagsCvAgv.D2026, "CV_Out SafetyCurtain (D2026)"),
            // OUTBOUND AGV→CV (write)
            ["out_agv_ready"]     = (OpcTagsCvAgv.D2111, "AGV_Out Ready (D2111)"),
            ["out_agv_arrived"]   = (OpcTagsCvAgv.D2112, "AGV_Out Arrived (D2112)"),
            ["out_agv_moving"]    = (OpcTagsCvAgv.D2113, "AGV_Out MovingOrCarrying (D2113)"),
            ["out_agv_intrusion"] = (OpcTagsCvAgv.D2114, "AGV_Out Intrusion (D2114)"),
            ["out_agv_receiving"] = (OpcTagsCvAgv.D2115, "AGV_Out ArmOrReceive (D2115)"),
            ["out_agv_qr_done"]   = (OpcTagsCvAgv.D2411, "AGV_Out QR Finished (D2411)"),
            ["out_cv_qr_ready"]   = (OpcTagsCvAgv.D2321, "CV_Out QR Ready (D2321)"),
        };

        var result = new Dictionary<string, object>();
        foreach (var (key, (nodeId, label)) in tags)
        {
            try
            {
                var dv = await _opc.ReadValueAsync(nodeId);
                var raw = Convert.ToInt32(dv.Value);
                result[key] = new
                {
                    nodeId,
                    label,
                    raw,
                    isTrue = raw != 0,
                    isGood = Opc.Ua.StatusCode.IsGood(dv.StatusCode),
                    timestamp = dv.SourceTimestamp == DateTime.MinValue ? DateTime.Now : dv.SourceTimestamp
                };
            }
            catch (Exception ex)
            {
                result[key] = new
                {
                    nodeId,
                    label,
                    raw = (int?)null,
                    isTrue = false,
                    isGood = false,
                    error = ex.Message,
                    timestamp = DateTime.Now
                };
            }
        }

        // Phân tích điều kiện handshake
        bool GetBool(string k) => result.TryGetValue(k, out var v) &&
            v is { } obj && (bool)(obj.GetType().GetProperty("isTrue")?.GetValue(obj) ?? false);

        bool GetNoBox(string k) => result.TryGetValue(k, out var v) &&
            v is { } obj && (int)(obj.GetType().GetProperty("raw")?.GetValue(obj) ?? 1) == 0;

        var analysis = new
        {
            inbound = new
            {
                canStartHandshake = GetBool("in_cv_ready") && GetBool("in_cv_allow") && GetBool("in_cv_safety") && GetNoBox("in_cv_has_box"),
                cvReady     = GetBool("in_cv_ready"),
                cvAllow     = GetBool("in_cv_allow"),
                cvSafety    = GetBool("in_cv_safety"),
                cvConveyor  = GetBool("in_cv_conveyor"),
                cvCraneReady = GetBool("in_cv_crane_ready"),
                cvHasBox    = GetBool("in_cv_has_box"),
                cvHasNoBox  = GetNoBox("in_cv_has_box"),
                cvHasNoBox2 = GetNoBox("in_cv_has_box2"),
                cvCurtainMuted = GetBool("in_cv_curtain"),
                canCallCrane = GetBool("in_cv_crane_ready"),
                agvReady    = GetBool("in_agv_ready"),
                agvArrived  = GetBool("in_agv_arrived"),
                inProgress  = GetBool("in_agv_inprogress"),
                intrusion   = GetBool("in_agv_intrusion"),
                qrReady     = GetBool("in_cv_qr_ready"),
            },
            outbound = new
            {
                canStartHandshake = GetBool("out_cv_ready") && GetBool("out_cv_request") && GetBool("out_cv_safety"),
                cvReady     = GetBool("out_cv_ready"),
                cvRequest   = GetBool("out_cv_request"),
                cvSafety    = GetBool("out_cv_safety"),
                cvConveyor  = GetBool("out_cv_conveyor"),
                cvCraneReady = GetBool("out_cv_crane_ready"),
                cvHasBox    = GetBool("out_cv_has_box"),
                cvHasBox2   = GetBool("out_cv_has_box2"),
                cvCurtainMuted = GetBool("out_cv_curtain"),
                canStartAgvPick = GetBool("out_cv_has_box"),
                agvReady    = GetBool("out_agv_ready"),
                agvArrived  = GetBool("out_agv_arrived"),
                agvMoving   = GetBool("out_agv_moving"),
                intrusion   = GetBool("out_agv_intrusion"),
                agvReceiving = GetBool("out_agv_receiving"),
                qrReady     = GetBool("out_cv_qr_ready"),
            }
        };

        return Ok(new { timestamp = DateTime.Now, tags = result, analysis });
    }

    /// <summary>
    /// [Inbound] Chờ D2021=1 – cassette sẵn sàng cho crane lấy
    /// </summary>
    [HttpPost("inbound/wait-crane-ready")]
    public async Task<IActionResult> Inbound_WaitCraneReady([FromQuery] int timeoutSeconds = 120)
    {
        return await RunStep("Inbound_WaitCraneReady", async ct =>
        {
            await _hs.WaitInboundCraneReadyAsync(TimeSpan.FromSeconds(timeoutSeconds), ct);
        });
    }

    /// <summary>
    /// [Outbound] Chờ D2022=1 – crane đã hạ hàng xuống băng tải
    /// </summary>
    [HttpPost("outbound/wait-crane-ready")]
    public async Task<IActionResult> Outbound_WaitCraneReady([FromQuery] int timeoutSeconds = 120)
    {
        return await RunStep("Outbound_WaitCraneReady", async ct =>
        {
            await _hs.WaitOutboundCraneReadyAsync(TimeSpan.FromSeconds(timeoutSeconds), ct);
        });
    }

    /// <summary>
    /// [Outbound] Chờ D2027=1 – cảm biến vật lý báo có hàng tại cửa kho xuất
    /// </summary>
    [HttpPost("outbound/wait-has-box")]
    public async Task<IActionResult> Outbound_WaitHasBox([FromQuery] int timeoutSeconds = 120)
    {
        return await RunStep("Outbound_WaitHasBox", async ct =>
        {
            await _hs.WaitOutboundHasBoxAsync(TimeSpan.FromSeconds(timeoutSeconds), ct);
        });
    }

    /// <summary>
    /// [Outbound] Chờ D2321=1 – PLC đã quét QR, WCS có thể đọc
    /// </summary>
    [HttpPost("outbound/wait-qr-ready")]
    public async Task<IActionResult> Outbound_WaitQrReady([FromQuery] int timeoutSeconds = 120)
    {
        return await RunStep("Outbound_WaitQrReady", async ct =>
        {
            await _hs.WaitOutboundQrReadyAsync(TimeSpan.FromSeconds(timeoutSeconds), ct);
        });
    }

    /// <summary>
    /// Ghi trực tiếp 1 tag boolean (dùng để test thủ công)
    /// </summary>
    [HttpPost("tags/write")]
    public async Task<IActionResult> WriteTag([FromBody] StationTagWriteRequest request)
    {
        try
        {
            var value = request.Value ? 1 : 0;
            await _opc.WriteValueAsync(request.NodeId, value);
            _logger.LogInformation("Manual write tag {NodeId} = {Value}", request.NodeId, value);
            return Ok(new { success = true, nodeId = request.NodeId, value, timestamp = DateTime.Now });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lỗi ghi tag {NodeId}", request.NodeId);
            return Ok(new { success = false, nodeId = request.NodeId, error = ex.Message, timestamp = DateTime.Now });
        }
    }

    // ==================================================================
    //  Internal helper
    // ==================================================================
    private async Task<IActionResult> RunStep(string stepName, Func<CancellationToken, Task> action)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(HttpContext.RequestAborted);
        cts.CancelAfter(TimeSpan.FromSeconds(60));
        try
        {
            _logger.LogInformation("Station handshake step: {Step}", stepName);
            await action(cts.Token);
            return Ok(new { success = true, step = stepName, timestamp = DateTime.Now });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Station handshake timeout/cancel: {Step}", stepName);
            return Ok(new { success = false, step = stepName, error = "Timeout hoặc bị hủy – CV chưa đáp ứng điều kiện trong thời gian quy định", timestamp = DateTime.Now });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi station handshake step: {Step}", stepName);
            return Ok(new { success = false, step = stepName, error = ex.Message, timestamp = DateTime.Now });
        }
    }
}

public class StationTagWriteRequest
{
    public string NodeId { get; set; } = string.Empty;
    public bool Value { get; set; }
}
