using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Api.Services;
using Wcs.Infrastructure.Data.Models;
using Wcs.Okamura;
using Wcs.Okamura.Enums;
using Wcs.Okamura.Models;
using Wcs.Okamura.Services;

namespace Wcs.Api.Controllers;
/// Controller xử lý các lệnh điều khiển Crane (thủ công / test)
/// </summary>
[ApiController]
[Route("api/crane")]
public class CraneController(
    ILogger<CraneController> logger,
    ICraneService craneService,
    IStationHandshakeService stationHandshakeService,
    IWmsService wmsService,
    IManualInboundService manualInboundService,
    IManualOutboundService manualOutboundService,
    IManualCraneTaskTracker manualCraneTaskTracker,
    ICraneTaskNoGenerator craneTaskNoGenerator,
    CraneTaskDispatchService craneTaskDispatchService) : ControllerBase
{
    private readonly ILogger<CraneController> _logger = logger;
    private readonly ICraneService _craneService = craneService;
    private readonly IStationHandshakeService _stationHandshakeService = stationHandshakeService;
    private readonly IWmsService _wmsService = wmsService;
    private readonly IManualInboundService _manualInboundService = manualInboundService;
    private readonly IManualOutboundService _manualOutboundService = manualOutboundService;
    private readonly IManualCraneTaskTracker _manualCraneTaskTracker = manualCraneTaskTracker;
    private readonly ICraneTaskNoGenerator _craneTaskNoGenerator = craneTaskNoGenerator;
    private readonly CraneTaskDispatchService _craneTaskDispatchService = craneTaskDispatchService;

    /// <summary>
    /// Bật/tắt chế độ nhập kho thủ công (rèm an toàn).
    /// open=1: BeforeDrop → EnterDropPoint → PutdownRack (D2024=1, mute rèm).
    /// open=0: BackToDropWaitingPoint → AfterDrop (đóng rèm, reset tag).
    /// Sau 5 phút nếu rèm vẫn mute thì WCS tự gọi đóng.
    /// </summary>
    [HttpPost("inbound/manual")]
    [ProducesResponseType(typeof(BaseResponse<ManualInboundResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<ManualInboundResult>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<ManualInboundResult>>> SetInboundManualMode(
        [FromBody] ManualInboundRequest request,
        [FromQuery] int timeoutSeconds = 5)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(BaseResponse<ManualInboundResult>.ErrorResult("Dữ liệu request không hợp lệ", "VALIDATION_ERROR", errors));
        }

        var ct = HttpContext.RequestAborted;

        try
        {
            var result = await _manualInboundService.SetManualModeAsync(request.Open, timeoutSeconds, ct);
            var message = request.Open == 1
                ? $"Đã bật chế độ thủ công (D2024={result.SafetySensorStatus}). Hết hạn {result.ExpiresAt:HH:mm:ss} UTC. Đặt hàng vào bệ rồi gọi POST /api/crane/inbound."
                : $"Đã tắt chế độ thủ công (D2024={result.SafetySensorStatus}).";
            return Ok(BaseResponse<ManualInboundResult>.SuccessResult(result, message));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (OpcConditionsNotMetException ex)
        {
            _logger.LogWarning("[Inbound Manual] Điều kiện OPC chưa đáp ứng khi open={Open}", request.Open);
            return BadRequest(BaseResponse<ManualInboundResult>.ErrorResult(
                ex.Message,
                "OPC_CONDITION_NOT_MET",
                ex.Result.ToErrorMessages()));
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[Inbound Manual] Timeout khi bật chế độ thủ công (chờ D2001+D2002+D2003)");
            if (request.Open == 1)
            {
                _logger.LogWarning("[Inbound Manual] Open timeout – tự động gọi close để đảm bảo an toàn");
                try { await _manualInboundService.SetManualModeAsync(0, timeoutSeconds, CancellationToken.None); }
                catch (Exception closeEx) { _logger.LogError(closeEx, "[Inbound Manual] Lỗi khi tự động close sau timeout"); }
            }
            return BadRequest(BaseResponse<ManualInboundResult>.ErrorResult(
                "Timeout chờ CV sẵn sàng (D2001+D2002+D2003). Kiểm tra rèm/PLC.",
                "HANDSHAKE_TIMEOUT"));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(BaseResponse<ManualInboundResult>.ErrorResult(ex.Message, "VALIDATION_ERROR"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Inbound Manual] Lỗi open={Open}", request.Open);
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<ManualInboundResult>.ErrorResult("Đã xảy ra lỗi khi xử lý chế độ thủ công", "INTERNAL_ERROR"));
        }
    }

    /// <summary>
    /// Đọc trạng thái rèm an toàn nhập kho (D2024).
    /// </summary>
    [HttpGet("inbound/manual/status")]
    [ProducesResponseType(typeof(BaseResponse<ManualInboundStatusResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<ManualInboundStatusResult>>> GetInboundManualStatus()
    {
        var result = await _manualInboundService.GetStatusAsync(HttpContext.RequestAborted);
        var message = result.IsGood
            ? $"D2024={result.SafetySensorStatus} – {result.Description}"
            : result.Error ?? "Không đọc được D2024";
        return Ok(BaseResponse<ManualInboundStatusResult>.SuccessResult(result, message));
    }

    /// <summary>
    /// Gửi lệnh nhập kho thủ công (Inbound).
    /// Flow: HandshakeAfterDrop → [WMS Confirm] → Crane Inbound → [WMS Complete]
    /// </summary>
    [HttpPost("inbound")]
    [ProducesResponseType(typeof(BaseResponse<CraneTaskResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CraneTaskResult>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<CraneTaskResult>>> SendInbound([FromBody] InboundTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(BaseResponse<CraneTaskResult>.ErrorResult("Dữ liệu request không hợp lệ", "VALIDATION_ERROR", errors));
        }

        var ct = HttpContext.RequestAborted;
        var positionCode = request.PutPosition.GetPositionCode();
        var taskNo = await _craneTaskNoGenerator.NextAsync(ct);

        try
        {
            // ── Step 1: Station handshake (báo băng tải chạy xong, rèm đóng lại) ───
            _logger.LogInformation("[Inbound] AfterDrop handshake. TaskNo={TaskNo}", taskNo);
            await _stationHandshakeService.Inbound_BackToDropWaitingPointAsync(ct);
            await _stationHandshakeService.Inbound_AfterDropAsync(ct, waitCraneReady: false);

            var craneReadyCheck = await _stationHandshakeService.CheckInboundCraneReadyAsync(ct);
            if (!craneReadyCheck.IsSatisfied)
                return OpcConditionBadRequest<CraneTaskResult>(craneReadyCheck);

            // ── Step 1.5: Đọc QR code từ OPC (kiểm tra ngay, không chờ) ──
            var cassetteId = request.CassetteId;
            if (string.IsNullOrEmpty(cassetteId))
            {
                _logger.LogInformation("[Inbound] Kiểm tra QR sẵn sàng (D2361=1). TaskNo={TaskNo}", taskNo);
                var qrReadyCheck = await _stationHandshakeService.CheckInboundQrReadyAsync(ct);
                if (!qrReadyCheck.IsSatisfied)
                    return OpcConditionBadRequest<CraneTaskResult>(qrReadyCheck);

                _logger.LogInformation("[Inbound] Đọc CassetteId từ OPC QR scanner. TaskNo={TaskNo}", taskNo);
                cassetteId = await _stationHandshakeService.Inbound_ReadQrCodeAsync(ct);
                if (string.IsNullOrEmpty(cassetteId))
                    _logger.LogWarning("[Inbound] Không đọc được QR code từ OPC (dữ liệu rỗng). TaskNo={TaskNo}", taskNo);
                else
                    _logger.LogInformation("[Inbound] Đọc QR code thành công: {CassetteId}. TaskNo={TaskNo}", cassetteId, taskNo);
            }

            // ── Step 2: WMS Confirm (đăng ký vị trí nhập kho với WMS) ──────────────
            if (request.CallWmsConfirm)
            {
                _logger.LogInformation("[Inbound] WMS ConfirmPosition {Position}", positionCode);
                await _wmsService.ConfirmPositionInboundAsync(
                    position:   positionCode,
                    stageCode:  request.StageCode  ?? string.Empty,
                    cassetteId: cassetteId          ?? string.Empty,
                    size:       request.Size        ?? string.Empty,
                    quantity:   request.Quantity,
                    product:    request.Product     ?? string.Empty,
                    ct:         ct);
            }

            // ── Step 3: Crane – gửi lệnh nhập kho (kiểm tra D2021, không chờ) ───
            _logger.LogInformation("[Inbound] Kiểm tra CraneReady (D2021=1). TaskNo={TaskNo}", taskNo);
            craneReadyCheck = await _stationHandshakeService.CheckInboundCraneReadyAsync(ct);
            if (!craneReadyCheck.IsSatisfied)
                return OpcConditionBadRequest<CraneTaskResult>(craneReadyCheck);

            _logger.LogInformation("[Inbound] Crane SendInbound Pick={Pick} Put={Put} TaskNo={TaskNo}",
                CraneStationPositions.Pick, positionCode, taskNo);

            await _craneTaskDispatchService.ReserveManualDispatchAsync(
                taskNo,
                CraneTaskType.Inbound,
                $"Manual inbound put={positionCode}",
                ct);

            // Track before sending. A fast PLC completion can arrive before SendInboundAsync returns.
            _manualCraneTaskTracker.TrackInbound(
                taskNo,
                positionCode,
                request.CallWmsComplete,
                request.StageCode ?? string.Empty,
                cassetteId ?? string.Empty,
                request.Size ?? string.Empty,
                request.Quantity);

            var craneResult = await _craneService.SendInboundAsync(
                new InboundTask(
                    taskNo,
                    CraneStationPositions.Pick,
                    new CranePosition(request.PutPosition.Row, request.PutPosition.Floor, request.PutPosition.Position)),
                ct,
                waitForReady: false);

            if (!craneResult.Success)
            {
                _manualCraneTaskTracker.Remove(taskNo);
                await _craneTaskDispatchService.MarkFailedAsync(
                    taskNo,
                    craneResult.ErrorMessage ?? "Crane inbound dispatch failed",
                    craneResult.ErrorCode,
                    ct);
                _logger.LogWarning("[Inbound] Crane FAIL TaskNo={TaskNo} Error={Error}", taskNo, craneResult.ErrorMessage);
                return BadRequest(BaseResponse<CraneTaskResult>.ErrorResult(
                    craneResult.ErrorMessage ?? "Crane từ chối lệnh nhập kho",
                    $"CRANE_ERROR_{craneResult.ErrorCode}",
                    craneResult.ErrorMessage != null ? [craneResult.ErrorMessage] : null));
            }

            await _craneTaskDispatchService.MarkAcceptedAsync(taskNo, ct);
            _logger.LogInformation("[Inbound] Crane accepted task. TaskNo={TaskNo} Position={Position}", taskNo, positionCode);
            return Ok(BaseResponse<CraneTaskResult>.SuccessResult(craneResult, $"Crane accepted inbound task. Position={positionCode}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Inbound] Lỗi. TaskNo={TaskNo}", taskNo);
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<CraneTaskResult>.ErrorResult("Đã xảy ra lỗi khi xử lý yêu cầu", "INTERNAL_ERROR"));
        }
    }

    /// <summary>
    /// Gửi lệnh xuất kho thủ công (Outbound).
    /// Flow: [WMS Confirm] → Crane Outbound → [WMS Complete]
    /// (Handshake đóng rèm qua POST outbound/manual open=0 sau khi công nhân lấy hàng)
    /// </summary>
    [HttpPost("outbound/manual")]
    [ProducesResponseType(typeof(BaseResponse<ManualOutboundResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<ManualOutboundResult>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<ManualOutboundResult>>> SetOutboundManualMode(
        [FromBody] ManualInboundRequest request,
        [FromQuery] int timeoutSeconds = 5)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(BaseResponse<ManualOutboundResult>.ErrorResult("Dữ liệu request không hợp lệ", "VALIDATION_ERROR", errors));
        }

        var ct = HttpContext.RequestAborted;

        try
        {
            var result = await _manualOutboundService.SetManualModeAsync(request.Open, timeoutSeconds, ct);
            var message = request.Open == 1
                ? $"Đã bật chế độ thủ công (D2026={result.SafetySensorStatus}). Hết hạn {result.ExpiresAt:HH:mm:ss} UTC. Gọi crane rồi lấy hàng, sau đó open=0."
                : $"Đã tắt chế độ thủ công (D2026={result.SafetySensorStatus}).";
            return Ok(BaseResponse<ManualOutboundResult>.SuccessResult(result, message));
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (OpcConditionsNotMetException ex)
        {
            _logger.LogWarning("[Outbound Manual] Điều kiện OPC chưa đáp ứng khi open={Open}", request.Open);
            return BadRequest(BaseResponse<ManualOutboundResult>.ErrorResult(
                ex.Message,
                "OPC_CONDITION_NOT_MET",
                ex.Result.ToErrorMessages()));
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("[Outbound Manual] Timeout khi bật chế độ thủ công (chờ D2011+D2012+D2013)");
            if (request.Open == 1)
            {
                _logger.LogWarning("[Outbound Manual] Open timeout – tự động gọi close để đảm bảo an toàn");
                try { await _manualOutboundService.SetManualModeAsync(0, timeoutSeconds, CancellationToken.None); }
                catch (Exception closeEx) { _logger.LogError(closeEx, "[Outbound Manual] Lỗi khi tự động close sau timeout"); }
            }
            return BadRequest(BaseResponse<ManualOutboundResult>.ErrorResult(
                "Timeout chờ CV sẵn sàng (D2011+D2012+D2013). Kiểm tra rèm/PLC.",
                "HANDSHAKE_TIMEOUT"));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(BaseResponse<ManualOutboundResult>.ErrorResult(ex.Message, "VALIDATION_ERROR"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Outbound Manual] Lỗi open={Open}", request.Open);
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<ManualOutboundResult>.ErrorResult("Đã xảy ra lỗi khi xử lý chế độ thủ công", "INTERNAL_ERROR"));
        }
    }

    /// <summary>
    /// Đọc trạng thái rèm an toàn xuất kho (D2026).
    /// </summary>
    [HttpGet("outbound/manual/status")]
    [ProducesResponseType(typeof(BaseResponse<ManualOutboundStatusResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<ManualOutboundStatusResult>>> GetOutboundManualStatus()
    {
        var result = await _manualOutboundService.GetStatusAsync(HttpContext.RequestAborted);
        var message = result.IsGood
            ? $"D2026={result.SafetySensorStatus} – {result.Description}"
            : result.Error ?? "Không đọc được D2026";
        return Ok(BaseResponse<ManualOutboundStatusResult>.SuccessResult(result, message));
    }

    /// <summary>
    /// Gửi lệnh xuất kho thủ công (Outbound).
    /// Flow: [WMS Confirm] → Crane Outbound → [WMS Complete]
    /// </summary>
    [HttpPost("outbound")]
    [ProducesResponseType(typeof(BaseResponse<CraneTaskResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CraneTaskResult>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<CraneTaskResult>>> SendOutbound([FromBody] OutboundTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(BaseResponse<CraneTaskResult>.ErrorResult("Dữ liệu request không hợp lệ", "VALIDATION_ERROR", errors));
        }

        var ct = HttpContext.RequestAborted;
        var positionCode = request.PickPosition.GetPositionCode();
        var taskNo = await _craneTaskNoGenerator.NextAsync(ct);

        try
        {
            // ── Step 1: WMS Confirm (báo WMS sắp lấy hàng ra) ──────────────────────
            if (request.CallWmsConfirm)
            {
                _logger.LogInformation("[Outbound] WMS ConfirmPosition {Position}", positionCode);
                await _wmsService.ConfirmPositionOutboundAsync(positionCode, ct);
            }

            // ── Step 2: Crane – gửi lệnh xuất kho (kiểm tra điều kiện, không chờ) ─
            var cvReadyCheck = await _stationHandshakeService.CheckOutboundCraneReadyAsync(ct);
            if (!cvReadyCheck.IsSatisfied)
                return OpcConditionBadRequest<CraneTaskResult>(cvReadyCheck);

            _logger.LogInformation("[Outbound] Crane SendOutbound Pick={Pick} Put={Put} TaskNo={TaskNo}",
                positionCode, CraneStationPositions.Put, taskNo);

            await _craneTaskDispatchService.ReserveManualDispatchAsync(
                taskNo,
                CraneTaskType.Outbound,
                $"Manual outbound pick={positionCode}",
                ct);

            var craneResult = await _craneService.SendOutboundAsync(
                new OutboundTask(
                    taskNo,
                    new CranePosition(request.PickPosition.Row, request.PickPosition.Floor, request.PickPosition.Position),
                    CraneStationPositions.Put),
                ct,
                waitForReady: false);

            if (!craneResult.Success)
            {
                await _craneTaskDispatchService.MarkFailedAsync(
                    taskNo,
                    craneResult.ErrorMessage ?? "Crane outbound dispatch failed",
                    craneResult.ErrorCode,
                    ct);
                _logger.LogWarning("[Outbound] Crane FAIL TaskNo={TaskNo} Error={Error}", taskNo, craneResult.ErrorMessage);
                return BadRequest(BaseResponse<CraneTaskResult>.ErrorResult(
                    craneResult.ErrorMessage ?? "Crane từ chối lệnh xuất kho",
                    $"CRANE_ERROR_{craneResult.ErrorCode}",
                    craneResult.ErrorMessage != null ? [craneResult.ErrorMessage] : null));
            }

            await _craneTaskDispatchService.MarkAcceptedAsync(taskNo, ct);

            // ── Step 3: WMS Complete ────────────────────────────────────────────────
            if (request.CallWmsComplete)
            {
                _logger.LogInformation("[Outbound] WMS CompleteTransfer {Position}", positionCode);
                await _wmsService.CompleteTransferOutboundAsync(positionCode, ct);
            }

            await _craneTaskDispatchService.MarkCompletedAsync(taskNo, ct);
            _logger.LogInformation("[Outbound] Hoàn thành. TaskNo={TaskNo} Position={Position}", taskNo, positionCode);
            return Ok(BaseResponse<CraneTaskResult>.SuccessResult(craneResult, $"Xuất kho thành công. Position={positionCode}"));
        }
        catch (WmsApiException ex)
        {
            _logger.LogWarning(
                "[Outbound] WMS rejected request. TaskNo={TaskNo}, Path={Path}, StatusCode={StatusCode}",
                taskNo,
                ex.Path,
                ex.StatusCode);

            return BadRequest(BaseResponse<CraneTaskResult>.ErrorResult(
                "WMS tu choi yeu cau xuat kho. Kiem tra log WMS response body de biet chi tiet.",
                "WMS_OUTBOUND_ERROR"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Outbound] Lỗi. TaskNo={TaskNo}", taskNo);
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<CraneTaskResult>.ErrorResult("Đã xảy ra lỗi khi xử lý yêu cầu", "INTERNAL_ERROR"));
        }
    }

    /// <summary>
    /// Gửi lệnh di chuyển nội bộ trong kho (Internal Move).
    /// Flow: Crane InternalMove (Pick → Put trong kho)
    /// </summary>
    [HttpPost("internal-move")]
    [ProducesResponseType(typeof(BaseResponse<CraneTaskResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<CraneTaskResult>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<CraneTaskResult>>> SendInternalMove([FromBody] InternalMoveTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(BaseResponse<CraneTaskResult>.ErrorResult("Dữ liệu request không hợp lệ", "VALIDATION_ERROR", errors));
        }

        var taskNo = await _craneTaskNoGenerator.NextAsync(HttpContext.RequestAborted);
        var pickCode = request.PickPosition.GetPositionCode();
        var putCode = request.PutPosition.GetPositionCode();
        var ct = HttpContext.RequestAborted;

        try
        {
            _logger.LogInformation("[InternalMove] Crane SendInternalMove Pick={Pick} Put={Put} TaskNo={TaskNo}",
                pickCode, putCode, taskNo);

            await _craneTaskDispatchService.ReserveManualDispatchAsync(
                taskNo,
                CraneTaskType.InternalMove,
                $"Manual internal move pick={pickCode} put={putCode}",
                ct);

            var craneResult = await _craneService.SendInternalMoveAsync(
                new InternalMoveTask(
                    taskNo,
                    new CranePosition(request.PickPosition.Row, request.PickPosition.Floor, request.PickPosition.Position),
                    new CranePosition(request.PutPosition.Row, request.PutPosition.Floor, request.PutPosition.Position)),
                HttpContext.RequestAborted,
                waitForReady: false);

            if (!craneResult.Success)
            {
                await _craneTaskDispatchService.MarkFailedAsync(
                    taskNo,
                    craneResult.ErrorMessage ?? "Crane internal move dispatch failed",
                    craneResult.ErrorCode,
                    ct);
                _logger.LogWarning("[InternalMove] Crane FAIL TaskNo={TaskNo} Error={Error}", taskNo, craneResult.ErrorMessage);
                return BadRequest(BaseResponse<CraneTaskResult>.ErrorResult(
                    craneResult.ErrorMessage ?? "Crane từ chối lệnh di chuyển nội bộ",
                    $"CRANE_ERROR_{craneResult.ErrorCode}",
                    craneResult.ErrorMessage != null ? [craneResult.ErrorMessage] : null));
            }

            await _craneTaskDispatchService.MarkAcceptedAsync(taskNo, ct);
            _logger.LogInformation("[InternalMove] Hoàn thành. TaskNo={TaskNo} Pick={Pick} Put={Put}", taskNo, pickCode, putCode);
            return Ok(BaseResponse<CraneTaskResult>.SuccessResult(craneResult, $"Di chuyển nội bộ thành công. Pick={pickCode} Put={putCode}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[InternalMove] Lỗi. TaskNo={TaskNo}", taskNo);
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<CraneTaskResult>.ErrorResult("Đã xảy ra lỗi khi xử lý yêu cầu", "INTERNAL_ERROR"));
        }
    }

    /// <summary>
    /// Danh sách lệnh crane đã ghi vào DB (tự động + thủ công).
    /// </summary>
    [HttpGet("dispatches")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<CraneTaskDispatchDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<CraneTaskDispatchDto>>>> GetDispatches(
        [FromQuery] int limit = 100,
        [FromQuery] string? status = null,
        [FromQuery] string? taskType = null,
        [FromQuery] string? source = null,
        [FromQuery] bool activeOnly = false,
        CancellationToken ct = default)
    {
        CraneTaskDispatchStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(status)
            && Enum.TryParse<CraneTaskDispatchStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            statusFilter = parsedStatus;
        }

        CraneTaskType? taskTypeFilter = null;
        if (!string.IsNullOrWhiteSpace(taskType)
            && Enum.TryParse<CraneTaskType>(taskType, ignoreCase: true, out var parsedTaskType))
        {
            taskTypeFilter = parsedTaskType;
        }

        bool? manualOnly = source?.ToLowerInvariant() switch
        {
            "manual" => true,
            "automatic" or "auto" => false,
            _ => null
        };

        var rows = await _craneTaskDispatchService.ListRecentAsync(
            limit,
            statusFilter,
            taskTypeFilter,
            manualOnly,
            activeOnly,
            ct);

        var dtos = rows.Select(CraneTaskDispatchDto.FromDbModel).ToList();
        return Ok(BaseResponse<IReadOnlyList<CraneTaskDispatchDto>>.SuccessResult(
            dtos,
            $"Lấy {dtos.Count} lệnh crane"));
    }

    /// <summary>
    /// Đọc trạng thái sẵn sàng nhận lệnh mới của crane (D3050, D3053, D3051, D3057).
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(BaseResponse<CraneReadyStatus>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<CraneReadyStatus>>> GetReadyStatus()
    {
        var status = await _craneService.GetReadyStatusAsync(HttpContext.RequestAborted);
        var message = status.IsReady
            ? "Crane sẵn sàng nhận lệnh mới"
            : status.Reason ?? "Crane chưa sẵn sàng";
        return Ok(BaseResponse<CraneReadyStatus>.SuccessResult(status, message));
    }

    /// <summary>
    /// Xóa toàn bộ dữ liệu instruction task trên PLC (D3003–D3010, D3016).
    /// </summary>
    [HttpPost("clear-task-data")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<bool>>> ClearTaskData()
    {
        try
        {
            _logger.LogInformation("[Crane] Clear task data");
            await _craneService.ClearTaskDataAsync();
            return Ok(BaseResponse<bool>.SuccessResult(true, "Đã clear task data"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Crane] Lỗi clear task data");
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<bool>.ErrorResult("Đã xảy ra lỗi khi clear task data", "INTERNAL_ERROR"));
        }
    }

    /// <summary>
    /// Ghi thủ công D3013 (wcs_taskFinishAffirm).
    /// </summary>
    [HttpPost("task-finish-affirm")]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<bool>>> WriteTaskFinishAffirm([FromBody] TaskFinishAffirmRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(BaseResponse<bool>.ErrorResult("Dữ liệu request không hợp lệ", "VALIDATION_ERROR", errors));
        }

        try
        {
            _logger.LogInformation("[Crane] Write D3013={Value}", request.Value);
            await _craneService.WriteTaskFinishAffirmAsync(request.Value);
            return Ok(BaseResponse<bool>.SuccessResult(true, $"Đã ghi D3013={request.Value}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Crane] Lỗi ghi D3013");
            return StatusCode(StatusCodes.Status500InternalServerError,
                BaseResponse<bool>.ErrorResult("Đã xảy ra lỗi khi ghi D3013", "INTERNAL_ERROR"));
        }
    }

    private static ActionResult<BaseResponse<T>> OpcConditionBadRequest<T>(OpcConditionCheckResult result)
        => new BadRequestObjectResult(BaseResponse<T>.ErrorResult(
            "Điều kiện OPC chưa đáp ứng",
            "OPC_CONDITION_NOT_MET",
            result.ToErrorMessages()));
}
