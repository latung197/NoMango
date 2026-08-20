using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Api.OrderMatching;
using Wcs.Common.Entities;
using Wcs.Common.ValueObjects;

namespace Wcs.Api.Controllers;

[ApiController]
[Route("api/transfer-requests")]
public class TransferRequestController(TransferMatchingService matchingService) : ControllerBase
{
    private readonly TransferMatchingService _matchingService = matchingService;

    [HttpPost("send")]
    public async Task<ActionResult<BaseResponse<TransferRequestResponseDto>>> Send(
        SendTransferRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var result = await _matchingService.CreateSendAsync(new SendTransferRequest
            {
                FromStation = request.FromStation,
                ToStage = request.ToStage,
                RobotCode = request.RobotCode,
                IsEmptyTray = request.IsEmptyTray,
                CassetteCode = request.CassetteCode,
                StorageStageCode = request.StorageStageCode,
                Quantity = request.Quantity,
                Product = request.Product,
                Size = request.Size,
            }, ct);

            var message = GetSendMessage(result);

            return Ok(BaseResponse<TransferRequestResponseDto>.SuccessResult(
                TransferRequestResponseDto.FromEntity(result), message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<TransferRequestResponseDto>.ErrorResult(ex.Message, MapTransferErrorCode(ex.Message)));
        }
    }

    [HttpPost("receive")]
    public async Task<ActionResult<BaseResponse<TransferRequestResponseDto>>> Receive(
        ReceiveTransferRequestDto request,
        CancellationToken ct)
    {
        try
        {
            var result = await _matchingService.CreateReceiveAsync(new ReceiveTransferRequest
            {
                FromStage = request.FromStage,
                ToStation = request.ToStation,
                RobotCode = request.RobotCode,
                IsEmptyTray = request.IsEmptyTray,
                IsWipTray = request.IsWipTray,
                Size = request.Size,
            }, ct);

            var message = GetReceiveMessage(result);

            return Ok(BaseResponse<TransferRequestResponseDto>.SuccessResult(
                TransferRequestResponseDto.FromEntity(result), message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<TransferRequestResponseDto>.ErrorResult(ex.Message, MapTransferErrorCode(ex.Message)));
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<BaseResponse<TransferRequestResponseDto>>> Cancel(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _matchingService.CancelAsync(id, ct);
            return Ok(BaseResponse<TransferRequestResponseDto>.SuccessResult(
                TransferRequestResponseDto.FromEntity(result), "Hủy yêu cầu thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BaseResponse<TransferRequestResponseDto>.ErrorResult(ex.Message, MapTransferErrorCode(ex.Message)));
        }
    }

    [HttpGet("waiting")]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<TransferRequestResponseDto>>>> GetWaiting(
        [FromQuery] string? stage,
        CancellationToken ct)
    {
        var requests = string.IsNullOrWhiteSpace(stage)
            ? await _matchingService.GetAllWaitingAsync(ct)
            : await _matchingService.GetWaitingByStageAsync(stage, ct);
        var dtos = requests.Select(TransferRequestResponseDto.FromEntity).ToList();
        return Ok(BaseResponse<IReadOnlyList<TransferRequestResponseDto>>.SuccessResult(
            dtos, $"Lấy {dtos.Count} yêu cầu đang chờ hoặc đang xử lý"));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BaseResponse<TransferRequestResponseDto>>> GetById(Guid id, CancellationToken ct)
    {
        var request = await _matchingService.GetByIdAsync(id, ct);
        if (request is null)
        {
            return NotFound(BaseResponse<TransferRequestResponseDto>.ErrorResult(
                "Yêu cầu không tồn tại", "TRANSFER_REQUEST_NOT_FOUND"));
        }

        return Ok(BaseResponse<TransferRequestResponseDto>.SuccessResult(
            TransferRequestResponseDto.FromEntity(request), "Lấy yêu cầu thành công"));
    }

    private static string MapTransferErrorCode(string message)
    {
        if (message.Contains("đầy hàng", StringComparison.OrdinalIgnoreCase)
            || message.Contains("không còn chỗ trống", StringComparison.OrdinalIgnoreCase))
        {
            return "STATION_FULL";
        }

        if (message.Contains("lệnh robot", StringComparison.OrdinalIgnoreCase))
        {
            return "STATION_BUSY";
        }

        if (message.Contains("yêu cầu gửi hàng đang chờ", StringComparison.OrdinalIgnoreCase)
            || message.Contains("yêu cầu nhận hàng đang chờ", StringComparison.OrdinalIgnoreCase))
        {
            return "TRANSFER_REQUEST_PENDING";
        }

        return message == "SIZE_REQUIRED" ? "SIZE_REQUIRED" : "TRANSFER_REQUEST_ERROR";
    }

    private static string GetSendMessage(TransferRequest result) => result.Status switch
    {
        TransferRequestStatus.Waiting when result.WarehousePendingKind == WarehousePendingKind.Inbound
            => "Yêu cầu đang chờ cửa kho nhập",
        TransferRequestStatus.Waiting => "Yêu cầu gửi hàng đang chờ khớp lệnh",
        TransferRequestStatus.Completed => "Chuyển hàng vào kho thành công, robot đang thực hiện",
        _ => "Khớp lệnh thành công, robot đang thực hiện",
    };

    private static string GetReceiveMessage(TransferRequest result) => result.Status switch
    {
        TransferRequestStatus.Waiting when result.WarehousePendingKind == WarehousePendingKind.Outbound
            => "Yêu cầu đang chờ cửa kho xuất",
        TransferRequestStatus.Waiting => "Yêu cầu nhận hàng đang chờ khớp lệnh",
        TransferRequestStatus.Completed => "Lấy hàng từ kho thành công, robot đang thực hiện",
        _ => "Khớp lệnh thành công, robot đang thực hiện",
    };
}
