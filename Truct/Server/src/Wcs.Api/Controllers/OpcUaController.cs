using Microsoft.AspNetCore.Mvc;
using Wcs.Api.Configs;
using Wcs.Api.Controllers.DTOs;
using Wcs.Common.Abstractions;
using Wcs.OpcUa.Contracts;

namespace Wcs.Api.Controllers;

/// <summary>
/// Controller để quản lý kết nối và tương tác với OPC UA server
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
public class OpcUaController : ControllerBase
{
    private readonly IOpcUaClient _opcUaClient;
    private readonly IConfigService _configService;
    private readonly ILogger<OpcUaController> _logger;

    public OpcUaController(IOpcUaClient opcUaClient, IConfigService configService, ILogger<OpcUaController> logger)
    {
        _opcUaClient = opcUaClient;
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Kết nối đến OPC UA server
    /// </summary>
    /// <returns>Kết quả kết nối</returns>
    /// <response code="200">Kết nối thành công</response>
    /// <response code="500">Lỗi kết nối</response>
    [HttpPost("connect")]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Connect()
    {
        try
        {
            var result = await _opcUaClient.ConnectAsync();
            if (result)
            {
                return Ok(new { message = "Kết nối OPC UA thành công", connected = true });
            }
            return BadRequest(new { message = "Không thể kết nối OPC UA", connected = false });
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi kết nối OPC UA");
            return StatusCode(500, new { message = "Lỗi nội bộ server", error = ex.Message });
        }
    }

    /// <summary>
    /// Ngắt kết nối OPC UA server
    /// </summary>
    /// <returns>Kết quả ngắt kết nối</returns>
    /// <response code="200">Ngắt kết nối thành công</response>
    /// <response code="500">Lỗi ngắt kết nối</response>
    [HttpPost("disconnect")]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Disconnect()
    {
        try
        {
            await _opcUaClient.DisconnectAsync();
            return Ok(new { message = "Đã ngắt kết nối OPC UA", connected = false });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi ngắt kết nối OPC UA");
            return StatusCode(500, new { message = "Lỗi nội bộ server", error = ex.Message });
        }
    }

    /// <summary>
    /// Kiểm tra trạng thái kết nối OPC UA
    /// </summary>
    /// <returns>Trạng thái kết nối</returns>
    /// <response code="200">Trạng thái kết nối</response>
    /// <response code="500">Lỗi kiểm tra trạng thái</response>
    [HttpGet("status")]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetStatus()
    {
        try
        {
            var isConnected = await _opcUaClient.IsConnectedAsync();
            return Ok(new { connected = isConnected, timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi kiểm tra trạng thái kết nối");
            return StatusCode(500, new { message = "Lỗi nội bộ server", error = ex.Message });
        }
    }

    /// <summary>
    /// Lấy danh sách Tag OPC
    /// </summary>
    /// <param name="device"></param>
    /// <returns>Tag OPC của các thiết bị</returns>
    /// <response code="200">Danh sách Tag OPC</response>
    /// <response code="500">Lỗi lấy danh sách Tag OPC</response>
    [HttpGet("signals")]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSignals(string? device)
    {
        try
        {
            SignalConfig signals = _configService.GetFromFile<SignalConfig>("signal.json");
            //RobotConfig signalConfig = _configService.GetFromFile<RobotConfig>("robot.json");
            return Ok(signals);
            //return Ok(new { results = signalConfig, totalCount = 1 });
            //return robotConfig.Robots.FirstOrDefault(r => r.Code.Equals(code, StringComparison.CurrentCultureIgnoreCase));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi lấy danh sách Tag OPC");
            return StatusCode(500, new { message = "Lỗi nội bộ server", error = ex.Message });
        }
    }

    /// <summary>
    /// Đọc giá trị từ node OPC UA
    /// </summary>
    /// <param name="nodeId">ID của node cần đọc (ví dụ: ns=2;s=A_Process.Tag02NB_00041)</param>
    /// <returns>Giá trị của node</returns>
    /// <response code="200">Đọc giá trị thành công</response>
    /// <response code="500">Lỗi nội bộ server</response>
    [HttpGet("read/{nodeId}")]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReadValue(string nodeId)
    {
        try
        {
            var dataValue = await _opcUaClient.ReadValueAsync(nodeId);
            return Ok(new
            {
                nodeId,
                value = dataValue.Value,
                timestamp = dataValue.SourceTimestamp,
                statusCode = dataValue.StatusCode,
                success = Opc.Ua.StatusCode.IsGood(dataValue.StatusCode)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đọc giá trị từ node {NodeId}", nodeId);
            return StatusCode(500, new { message = "Lỗi khi đọc giá trị", error = ex.Message });
        }
    }

    /// <summary>
    /// Ghi giá trị boolean vào node OPC UA
    /// </summary>
    /// <param name="nodeId">ID của node cần ghi (ví dụ: ns=2;s=A_Process.Tag02NB_00041)</param>
    /// <param name="request">Request chứa giá trị boolean (0 hoặc 1)</param>
    /// <returns>Kết quả ghi giá trị</returns>
    /// <response code="200">Ghi giá trị thành công</response>
    /// <response code="400">Ghi giá trị thất bại hoặc giá trị không hợp lệ</response>
    /// <response code="500">Lỗi nội bộ server</response>
    [HttpPost("write/{nodeId}")]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest)]
    [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> WriteValue(string nodeId, [FromBody] OpcUaWriteValueRequest request)
    {
        try
        {
            
            var result = await _opcUaClient.WriteValueAsync(nodeId, request.Value);
            if (result)
            {
                return Ok(new { 
                    message = "Ghi giá trị thành công", 
                    nodeId, 
                    value = request.Value,
                });
            }
            return BadRequest(new { 
                message = "Ghi giá trị thất bại", 
                nodeId, 
                value = request.Value,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi ghi giá trị vào node {NodeId}", nodeId);
            return StatusCode(500, new { message = "Lỗi khi ghi giá trị", error = ex.Message });
        }
    }

    [HttpGet("read-multiple")]
    public async Task<IActionResult> ReadMultipleValues([FromQuery] List<string> nodeIds)
    {
        try
        {
            var dataValues = await _opcUaClient.ReadMultipleValuesAsync(nodeIds);
            var results = new List<object>();
            
            for (int i = 0; i < nodeIds.Count && i < dataValues.Count; i++)
            {
                results.Add(new
                {
                    nodeId = nodeIds[i],
                    value = dataValues[i].Value,
                    timestamp = dataValues[i].SourceTimestamp,
                    statusCode = dataValues[i].StatusCode,
                    success = Opc.Ua.StatusCode.IsGood(dataValues[i].StatusCode)
                });
            }
            
            return Ok(new { results = results, totalCount = results.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đọc nhiều giá trị");
            return StatusCode(500, new { message = "Lỗi khi đọc nhiều giá trị", error = ex.Message });
        }
    }

    [HttpGet("browse/{nodeId}")]
    public async Task<IActionResult> BrowseNode(string nodeId)
    {
        try
        {
            var references = await _opcUaClient.BrowseAsync(nodeId);
            var results = references.Select(reference => new
            {
                nodeId = reference.NodeId.ToString(),
                browseName = reference.BrowseName?.Name,
                displayName = reference.DisplayName?.Text,
                nodeClass = reference.NodeClass.ToString()
            }).ToList();
            
            return Ok(new { results = results, totalCount = results.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi browse node {NodeId}", nodeId);
            return StatusCode(500, new { message = "Lỗi khi browse node", error = ex.Message });
        }
    }
}
