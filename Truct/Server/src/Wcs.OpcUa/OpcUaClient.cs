using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Wcs.OpcUa.Contracts;
using Opc.Ua;
using Opc.Ua.Client;

namespace Wcs.OpcUa;

public class OpcUaClient(IOptions<OpcUaOptions> options, ILogger<OpcUaClient> logger) : IOpcUaClient, IDisposable
{
    private readonly OpcUaOptions _options = options.Value;
    private readonly ILogger<OpcUaClient> _logger = logger;
    private Session? _session;
    private ApplicationConfiguration? _configuration;
    private Subscription? _sharedSubscription;
    private bool _subscriptionCreated;
    private readonly Dictionary<string, MonitoredItem> _monitoredItems = new();
    private readonly Dictionary<string, Action<DataValue>> _callbacks = new();
    private bool _disposed = false;
    private readonly object _lockObject = new();

    private string EnsureNodeIdPrefix(string tagName)
    {
        if (string.IsNullOrWhiteSpace(tagName))
            return tagName;

        // Nếu tag name đã có prefix namespace, trả về như cũ
        if (tagName.StartsWith("ns=") || tagName.StartsWith("i=") || tagName.StartsWith("s="))
            return tagName;

        // Thêm prefix ns=2;s= vào trước tag name
        return $"ns=2;s={tagName}";
    }

    public async Task<bool> ConnectAsync()
    {
        try
        {
            lock (_lockObject)
            {
                if (_session?.Connected == true)
                {
                    _logger.LogInformation("OPC UA client đã được kết nối");
                    return true;
                }
            }
            var endpoint = await SelectEndpointAsync();
            _configuration = CreateApplicationConfiguration();
            var configuredEndpoint = new ConfiguredEndpoint(null, endpoint, EndpointConfiguration.Create(_configuration));
            
            _session = await Session.Create(_configuration, configuredEndpoint, false, _options.ApplicationName, 60000, null, null);

            ClearSubscriptionState();

            lock (_lockObject)
            {
                if (_session.Connected)
                {
                    _logger.LogInformation("Kết nối OPC UA thành công đến {EndpointUrl}", _options.EndpointUrl);
                    return true;
                }
            }
            
            _logger.LogError("Không thể kết nối đến OPC UA server");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError("Lỗi khi kết nối OPC UA");
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        try
        {
            Session? sessionToClose = null;
            lock (_lockObject)
            {
                if (_session?.Connected == true)
                {
                    sessionToClose = _session;
                }
            }

            if (sessionToClose != null)
            {
                await sessionToClose.CloseAsync();
                ClearSubscriptionState();
                _session = null;
                _logger.LogInformation("Đã ngắt kết nối OPC UA");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi ngắt kết nối OPC UA");
        }
    }

    public Task<bool> IsConnectedAsync()
    {
        lock (_lockObject)
        {
            return Task.FromResult(_session?.Connected == true);
        }
    }

    public async Task<DataValue> ReadValueAsync(string nodeId)
    {
        try
        {
            if (!await EnsureConnectedAsync())
                return new DataValue(StatusCodes.BadNotConnected);

            var fullNodeId = EnsureNodeIdPrefix(nodeId);
            var node = new NodeId(fullNodeId);
            var dataValue = await _session!.ReadValueAsync(node);
            _logger.LogDebug("Đọc giá trị từ node {NodeId}: {Value}", fullNodeId, dataValue.Value);
            return dataValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đọc giá trị từ node {NodeId}", nodeId);
            return new DataValue(StatusCodes.BadUnexpectedError);
        }
    }

    public async Task<bool> WriteValueAsync(string nodeId, int value)
    {
        try
        {
            if (!await EnsureConnectedAsync())
                return false;

            if (value < short.MinValue || value > short.MaxValue)
            {
                _logger.LogWarning("Giá trị {Value} vượt quá phạm vi của kiểu short khi ghi vào node {NodeId}", value, nodeId);
                return false;
            }

            var fullNodeId = EnsureNodeIdPrefix(nodeId);
            var node = new NodeId(fullNodeId);
            var dataValue = new DataValue(new Variant((short)value));
            var writeValue = new WriteValue
            {
                NodeId = node,
                AttributeId = Attributes.Value,
                Value = dataValue
            };
            
            var writeValues = new WriteValueCollection { writeValue };
            var response = await _session!.WriteAsync(null, writeValues, CancellationToken.None);
            
            if (response.Results.Count > 0 && StatusCode.IsGood(response.Results[0]))
            {
                _logger.LogDebug("Ghi giá trị {Value} vào node {NodeId} thành công", value, fullNodeId);
                return true;
            }
            
            _logger.LogWarning("Ghi giá trị vào node {NodeId} thất bại: {Status}", fullNodeId, response.Results.FirstOrDefault());
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi ghi giá trị vào node {NodeId}", nodeId);
            return false;
        }
    }

    public async Task<List<DataValue>> ReadMultipleValuesAsync(List<string> nodeIds)
    {
        try
        {
            if (!await EnsureConnectedAsync())
                return new List<DataValue>();

            var readValueIds = nodeIds.Select(id => new ReadValueId { NodeId = new NodeId(EnsureNodeIdPrefix(id)), AttributeId = Attributes.Value }).ToArray();
            var readValueIdCollection = new ReadValueIdCollection(readValueIds);
            var response = await _session!.ReadAsync(null, 0, TimestampsToReturn.Both, readValueIdCollection, CancellationToken.None);
            
            _logger.LogDebug("Đọc {Count} giá trị thành công", response.Results.Count);
            return response.Results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đọc nhiều giá trị");
            return new List<DataValue>();
        }
    }

    public async Task<bool> SubscribeAsync(string nodeId, Action<DataValue> callback)
    {
        try
        {
            if (!await EnsureConnectedAsync())
                return false;

            var fullNodeId = EnsureNodeIdPrefix(nodeId);

            lock (_lockObject)
            {
                if (_monitoredItems.ContainsKey(nodeId))
                {
                    _logger.LogWarning("Đã có subscription cho node {NodeId}", nodeId);
                    return false;
                }
            }

            var subscription = GetOrCreateSharedSubscription();
            var monitoredItem = CreateMonitoredItem(fullNodeId, callback);
            subscription.AddItem(monitoredItem);

            await ApplySubscriptionChangesAsync();

            lock (_lockObject)
            {
                _monitoredItems[nodeId] = monitoredItem;
                _callbacks[nodeId] = callback;
            }

            _logger.LogInformation("Đăng ký monitored item cho node {NodeId} thành công", fullNodeId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký subscription cho node {NodeId}", nodeId);
            return false;
        }
    }

    public async Task<bool> UnsubscribeAsync(string nodeId)
    {
        try
        {
            MonitoredItem? monitoredItem;
            lock (_lockObject)
            {
                if (!_monitoredItems.TryGetValue(nodeId, out monitoredItem))
                {
                    return false;
                }
            }

            if (_sharedSubscription != null && monitoredItem != null)
            {
                _sharedSubscription.RemoveItem(monitoredItem);

                var shouldRemoveSubscription = false;
                lock (_lockObject)
                {
                    _monitoredItems.Remove(nodeId);
                    _callbacks.Remove(nodeId);
                    shouldRemoveSubscription = _monitoredItems.Count == 0;
                }

                if (shouldRemoveSubscription)
                {
                    await RemoveSharedSubscriptionAsync();
                }
                else
                {
                    await ApplySubscriptionChangesAsync();
                }

                _logger.LogInformation("Hủy monitored item cho node {NodeId} thành công", nodeId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi hủy subscription cho node {NodeId}", nodeId);
            return false;
        }
    }

    public async Task<List<ReferenceDescription>> BrowseAsync(string nodeId)
    {
        try
        {
            if (!await EnsureConnectedAsync())
                return new List<ReferenceDescription>();

            var fullNodeId = EnsureNodeIdPrefix(nodeId);
            var node = new NodeId(fullNodeId);
            var (responseHeader, continuationPoints, referencesList, errors) = await _session!.BrowseAsync(null, null, new[] { node }, 0, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object);
            
            var result = new List<ReferenceDescription>();
            foreach (var reference in referencesList)
            {
                result.AddRange(reference);
            }
            
            _logger.LogDebug("Browse node {NodeId} thành công, tìm thấy {Count} references", fullNodeId, result.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi browse node {NodeId}", nodeId);
            return new List<ReferenceDescription>();
        }
    }

    public async Task<bool> SubscribeToAllNodesAsync(List<string> nodeIds, Action<string, DataValue> callback)
    {
        try
        {
            if (!await EnsureConnectedAsync())
                return false;

            var uniqueNodeIds = nodeIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .ToList();

            var successCount = 0;
            var failedCount = 0;
            var skippedCount = 0;

            _logger.LogInformation("Bắt đầu đăng ký subscription cho {Count} nodes (1 subscription dùng chung)", uniqueNodeIds.Count);

            EnsureSubscriptionSessionValid();
            var subscription = GetOrCreateSharedSubscription();
            var pendingItems = new List<(string NodeId, MonitoredItem Item)>();

            foreach (var nodeId in uniqueNodeIds)
            {
                lock (_lockObject)
                {
                    if (_monitoredItems.ContainsKey(nodeId))
                    {
                        skippedCount++;
                        successCount++;
                        continue;
                    }
                }

                try
                {
                    var fullNodeId = EnsureNodeIdPrefix(nodeId);
                    var monitoredItem = CreateMonitoredItem(fullNodeId, dataValue => callback(nodeId, dataValue));
                    subscription.AddItem(monitoredItem);
                    pendingItems.Add((nodeId, monitoredItem));
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _logger.LogError(ex, "Lỗi khi tạo monitored item cho node: {NodeId}", nodeId);
                }
            }

            if (pendingItems.Count > 0)
            {
                await ApplySubscriptionChangesAsync();

                lock (_lockObject)
                {
                    foreach (var (nodeId, item) in pendingItems)
                    {
                        _monitoredItems[nodeId] = item;
                        successCount++;
                        _logger.LogDebug("Đăng ký monitored item thành công cho node: {NodeId}", EnsureNodeIdPrefix(nodeId));
                    }
                }
            }

            _logger.LogInformation(
                "Kết quả đăng ký subscription: {SuccessCount} thành công ({SkippedCount} đã có), {FailedCount} thất bại",
                successCount, skippedCount, failedCount);
            return successCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng ký subscription cho tất cả nodes");
            return false;
        }
    }

    private void EnsureSubscriptionSessionValid()
    {
        lock (_lockObject)
        {
            if (_sharedSubscription != null && _sharedSubscription.Session != _session)
            {
                ClearSubscriptionState();
            }
        }
    }

    private Subscription GetOrCreateSharedSubscription()
    {
        if (_sharedSubscription != null && _sharedSubscription.Session == _session)
        {
            return _sharedSubscription;
        }

        _sharedSubscription = new Subscription
        {
            PublishingEnabled = true,
            Priority = 100,
            PublishingInterval = _options.PublishingIntervalMs,
            KeepAliveCount = (uint)(_options.KeepAliveIntervalMs / _options.PublishingIntervalMs)
        };

        _session!.AddSubscription(_sharedSubscription);
        _subscriptionCreated = false;
        return _sharedSubscription;
    }

    private MonitoredItem CreateMonitoredItem(string fullNodeId, Action<DataValue> callback)
    {
        var monitoredItem = new MonitoredItem
        {
            StartNodeId = new NodeId(fullNodeId),
            AttributeId = Attributes.Value,
            SamplingInterval = _options.SamplingIntervalMs,
            QueueSize = 10,
            DiscardOldest = true
        };

        monitoredItem.Notification += (_, e) =>
        {
            if (e.NotificationValue is MonitoredItemNotification notification)
            {
                var dataValue = notification.Value;
                callback(dataValue);
                _logger.LogDebug("Nhận notification từ node {NodeId}: {Value}", fullNodeId, dataValue.Value);
            }
        };

        return monitoredItem;
    }

    private async Task ApplySubscriptionChangesAsync()
    {
        if (_sharedSubscription == null)
        {
            return;
        }

        if (!_subscriptionCreated)
        {
            await _sharedSubscription.CreateAsync();
            _subscriptionCreated = true;
            _logger.LogInformation("Đã tạo shared subscription thành công");
        }
        else
        {
            await _sharedSubscription.ApplyChangesAsync();
        }
    }

    private async Task RemoveSharedSubscriptionAsync()
    {
        if (_sharedSubscription == null)
        {
            return;
        }

        try
        {
            await _sharedSubscription.DeleteAsync(false);
            _session?.RemoveSubscription(_sharedSubscription);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Lỗi khi xóa shared subscription");
        }
        finally
        {
            _sharedSubscription = null;
            _subscriptionCreated = false;
        }
    }

    private void ClearSubscriptionState()
    {
        lock (_lockObject)
        {
            _sharedSubscription = null;
            _subscriptionCreated = false;
            _monitoredItems.Clear();
            _callbacks.Clear();
        }
    }

    private ApplicationConfiguration CreateApplicationConfiguration()
    {
        var config = new ApplicationConfiguration
        {
            ApplicationName = _options.ApplicationName,
            ApplicationUri = $"urn:{_options.ApplicationName}:{Environment.MachineName}",
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = null,
            TransportConfigurations = [],
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
        };

        // await config.Validate(ApplicationType.Client);
        return config;
    }

  private Task<EndpointDescription> SelectEndpointAsync()
    {
        var endpoint = CoreClientUtils.SelectEndpoint(_options.EndpointUrl, useSecurity: false, 15000);
        _logger.LogInformation("Đã tìm thấy endpoint: {Endpoint}", endpoint);
        return Task.FromResult(endpoint);
    }

    private async Task<bool> EnsureConnectedAsync()
    {
        lock (_lockObject)
        {
            if (_session?.Connected == true)
            {
                return true;
            }
        }
        return await ConnectAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            lock (_lockObject)
            {
                if (!_disposed)
                {
                    _session?.Dispose();
                    _disposed = true;
                }
            }
        }
    }

    public async Task<bool> ReadBooleanValueAsync(string nodeId)
    {
        var dataValue = await ReadValueAsync(nodeId);
        return dataValue.Value as bool? ?? false;
    }
}
