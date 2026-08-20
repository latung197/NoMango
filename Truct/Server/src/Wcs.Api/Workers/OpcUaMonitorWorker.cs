using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Opc.Ua;
using Wcs.Api.Services;
using Wcs.Common.Abstractions;
using Wcs.Common.Entities;
using Wcs.Common.Events;
using Wcs.OpcUa;
using Wcs.OpcUa.Contracts;
using Wcs.Okamura;
using Wcs.Okamura.Services;

namespace Wcs.Api.Workers;

public class OpcUaMonitorWorker(
    IOpcUaClient opcUaClient,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<OpcUaMonitorWorker> logger,
    IOptions<OpcUaOptions> opcUaOptions,
    IEventPublisher eventPublisher,
    ICraneMonitorService craneMonitorService) : BackgroundService
{
    private readonly IOpcUaClient _opcUaClient = opcUaClient;
    private readonly ILogger<OpcUaMonitorWorker> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly IOptions<OpcUaOptions> _opcUaOptions = opcUaOptions;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly ICraneMonitorService _craneMonitorService = craneMonitorService;
    private readonly Dictionary<string, DataValue> _lastValues = new();
    private readonly object _lockObject = new();
    private bool _subscriptionsInitialized = false;

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Thiết lập event publisher cho CraneMonitorService
        _craneMonitorService.SetEventPublisher(async (eventObj) =>
        {
            await _eventPublisher.PublishAsync(eventObj);
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await _opcUaClient.IsConnectedAsync())
                {
                    _logger.LogInformation("Đang kết nối đến OPC UA server... {EndpointUrl}", _opcUaOptions.Value.EndpointUrl);
                    var connected = await _opcUaClient.ConnectAsync();
                    if (!connected)
                    {
                        _logger.LogWarning("Không thể kết nối đến OPC UA server, thử lại sau 30 giây...");
                        await Task.Delay(30000, stoppingToken);
                        continue;
                    }
                    
                    // Reset subscription flag khi kết nối mới
                    _subscriptionsInitialized = false;
                }

                // Chỉ đăng ký subscription một lần khi kết nối thành công
                if (!_subscriptionsInitialized)
                {
                    await SubscribeToAllNodesAsync();
                    _subscriptionsInitialized = true;
                }

                // Chờ một khoảng thời gian trước khi kiểm tra kết nối lại
                await Task.Delay(5000, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong OPC UA Monitor Worker");
                await Task.Delay(5000, stoppingToken);
            }
        }

        _logger.LogInformation("OPC UA Monitor Worker đang dừng...");
    }

    private async Task SubscribeToAllNodesAsync()
    {
        try
        {
            var nodeIds = new List<string> { _opcUaOptions.Value.TagClockPulseRead, _opcUaOptions.Value.TagSystemStatus };
            
            // Thêm các crane tags
            nodeIds.AddRange(_craneMonitorService.GetRequiredCraneTags());
            
            var stations = await GetStationsAsync();
            _logger.LogInformation("Tìm thấy {Count} stations", stations.Count());
            foreach (var station in stations)
            {
                _logger.LogInformation("Station {Code} có tag_HasCassette: {Tag_HasCassette}", station.Code, station.Tag_HasCassette);
                if (!string.IsNullOrEmpty(station.Tag_ConveyorState))
                {
                    nodeIds.Add(station.Tag_ConveyorState);
                }
                if (!string.IsNullOrEmpty(station.Tag_CurtainState))
                {
                    nodeIds.Add(station.Tag_CurtainState);
                }
                if (!string.IsNullOrEmpty(station.Tag_Status))
                {
                    nodeIds.Add(station.Tag_Status);
                }
                if (!string.IsNullOrEmpty(station.Tag_HasCassette))
                {
                    nodeIds.Add(station.Tag_HasCassette);
                }
                if (!string.IsNullOrEmpty(station.Tag_QRCode))
                {
                    nodeIds.Add(station.Tag_QRCode);
                }
                if (!string.IsNullOrEmpty(station.Tag_ConveyorNumber))
                {
                    nodeIds.Add(station.Tag_ConveyorNumber);
                }
                if (!string.IsNullOrEmpty(station.Tag_Control))
                {
                    nodeIds.Add(station.Tag_Control);
                }
            }
            _logger.LogInformation("OPC UA Monitor Worker đang khởi động...");
            _logger.LogInformation("Đang đăng ký subscription cho {Count} nodes", nodeIds.Count);
            if (nodeIds.Count == 0)
            {
                _logger.LogWarning("Không có node IDs nào để đăng ký subscription");
                return;
            }
            
            var success = await _opcUaClient.SubscribeToAllNodesAsync(nodeIds, OnNodeValueChangedAsync);
            if (success)
            {
                _logger.LogInformation("Đã đăng ký subscription cho {Count} nodes thành công", nodeIds.Count);
                await _craneMonitorService.InitializeCraneHeartbeatAsync();
            }
            else
            {
                _logger.LogWarning("Không thể đăng ký subscription cho tất cả nodes");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký subscription cho tất cả nodes");
        }
    }

    private async Task<IEnumerable<Station>> GetStationsAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var stationService = scope.ServiceProvider.GetRequiredService<StationService>();
        return await stationService.GetAllStations();
    }

    private void OnNodeValueChangedAsync(string nodeId, DataValue dataValue)
    {
        try
        {
            lock (_lockObject)
            {
                if (_lastValues.TryGetValue(nodeId, out var lastValue))
                {
                    if (!Equals(lastValue.Value, dataValue.Value))
                    {   
                        // Xử lý logic nghiệp vụ dựa trên thay đổi giá trị
                        ProcessNodeValueChange(nodeId, lastValue.Value, dataValue.Value);
                        
                        // Xử lý crane completion (D2053, D2056) - publish CraneTaskCompletedEvent
                        _craneMonitorService.ProcessCraneTaskCompletionAsync(nodeId, dataValue);
                        
                        // Xử lý crane error (D2051, D2052, D2054, D2050) - publish CraneErrorEvent
                        _craneMonitorService.ProcessCraneErrorAsync(nodeId, lastValue, dataValue);
                        // Xử lý CV↔AGV signal: D2022 → CraneOutboundComplete, D2027 → clear D2321, D2361 → CraneInboundReady
                        _craneMonitorService.ProcessCvAgvSignalAsync(nodeId, lastValue, dataValue);
                        _craneMonitorService.ProcessCraneHeartbeatAsync(nodeId, dataValue);
                    }
                }
                else
                {
                    _craneMonitorService.ProcessCraneHeartbeatAsync(nodeId, dataValue);
                    _logger.LogInformation("🆕 Nhận giá trị đầu tiên từ subscription cho node {NodeId}: {Value}", nodeId, dataValue.Value);
                }
                
                _lastValues[nodeId] = dataValue;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xử lý thay đổi giá trị node {NodeId}", nodeId);
        }
    }


    private async void ProcessNodeValueChange(string nodeId, object? oldValue, object? newValue)
    {
        try
        {
            // Publish event để EventProcessingWorker có thể xử lý
            var @event = new OpcTagChangedEvent(nodeId, oldValue, newValue);
            await _eventPublisher.PublishAsync(@event);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi publish event cho node {NodeId}", nodeId);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _opcUaClient.DisconnectAsync();
            _logger.LogInformation("Đã ngắt kết nối OPC UA");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi ngắt kết nối OPC UA");
        }
        
        await base.StopAsync(cancellationToken);
    }

    public Dictionary<string, DataValue> GetLastValues()
    {
        lock (_lockObject)
        {
            return new Dictionary<string, DataValue>(_lastValues);
        }
    }

    public DataValue? GetLastValue(string nodeId)
    {
        lock (_lockObject)
        {
            return _lastValues.TryGetValue(nodeId, out var value) ? value : null;
        }
    }

}
