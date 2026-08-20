using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Controllers.DTOs;
using Wcs.Common.Abstractions;
using Wcs.Common.Abstractions.Repositories;
using Wcs.Common.Events;

namespace Wcs.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RcsCallbackController(ILogger<RcsCallbackController> logger, IEventPublisher publisher, IFlowTaskRepository flowTaskRepository) : ControllerBase
{
    private readonly ILogger<RcsCallbackController> _logger = logger;
    private readonly IEventPublisher _publisher = publisher;
    private readonly IFlowTaskRepository _flowTaskRepository = flowTaskRepository;

    [HttpPost("/agv/agvCallbackService/agvCallback")]
    public async Task<RcsCallbackResponse> AgvCallback([FromBody] RcsCallbackRequest request)
    {
        this._logger.LogInformation("AGV callback received");
        
        // Tách request.Method thành số thứ tự và trạng thái
        var (sequenceNumber, status) = ParseMethod(request.Method);
        
        this._logger.LogInformation("RCS Callback - TaskCode: {TaskCode}, RobotCode: {RobotCode}, Method: {Method}, Sequence: {Sequence}, Status: {Status}", 
            request.TaskCode, request.RobotCode, request.Method, sequenceNumber, status);

        var flowTask = await _flowTaskRepository.GetByRcsTaskIdAsync(request.TaskCode);
        if (flowTask == null) {
            this._logger.LogWarning("FlowTask not found: {TaskCode}", request.TaskCode);
            return new RcsCallbackResponse
            {
                Code = "1",
                Data = null,
                Message = "FlowTask not found",
                ReqCode = request.ReqCode
            };
        }
        
        if (status == "Start") {
            await _publisher.PublishAsync(new RobotTaskBegin(flowTask.Id.ToString(), sequenceNumber));
        } else if (status == "Complete") {
            await _publisher.PublishAsync(new RobotTaskComplete(flowTask.Id.ToString(), sequenceNumber));
        }
        
        return new RcsCallbackResponse
        {
            Code = "0",
            Data = null,
            Message = "successful",
            ReqCode = request.ReqCode
        };
    }

    [HttpPost("/agv/agvCallbackService/warnCallback")]
    public RcsCallbackResponse WarnCallback([FromBody] RcsWarnCallbackRequest request)
    {
        this._logger.LogInformation(
            "RCS warnCallback received - ReqCode: {ReqCode}, ReqTime: {ReqTime}, ClientCode: {ClientCode}, TokenCode: {TokenCode}, WarnCount: {WarnCount}",
            request.ReqCode, request.ReqTime, request.ClientCode, request.TokenCode, request.Data?.Count ?? 0);

        if (request.Data != null)
        {
            foreach (var warn in request.Data)
            {
                this._logger.LogWarning(
                    "RCS warnCallback - RobotCode: {RobotCode}, BeginTime: {BeginTime}, WarnContent: {WarnContent}, TaskCode: {TaskCode}",
                    warn.RobotCode, warn.BeginTime, warn.WarnContent, warn.TaskCode);
            }
        }

        return new RcsCallbackResponse
        {
            Code = "0",
            Data = null,
            Message = "successful",
            ReqCode = request.ReqCode
        };
    }

    /// <summary>
    /// Tách request.Method có dạng "1_Complete", "1_Cancel" thành số thứ tự và trạng thái
    /// </summary>
    /// <param name="method">Chuỗi method cần tách, ví dụ: "1_Complete", "2_Cancel"</param>
    /// <returns>Tuple chứa số thứ tự và trạng thái</returns>
    private (int sequenceNumber, string status) ParseMethod(string method)
    {
        if (string.IsNullOrEmpty(method))
        {
            return (0, string.Empty);
        }

        var parts = method.Split('_', 2);
        
        if (parts.Length != 2)
        {
            this._logger.LogWarning("Invalid method format: {Method}. Expected format: 'number_status'", method);
            return (0, method);
        }

        if (int.TryParse(parts[0], out int sequenceNumber))
        {
            return (sequenceNumber, parts[1]);
        }
        else
        {
            this._logger.LogWarning("Invalid sequence number in method: {Method}. Expected numeric prefix.", method);
            return (0, parts[1]);
        }
    }
}

