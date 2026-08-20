using Opc.Ua;

namespace Wcs.OpcUa.Contracts;

public interface IOpcUaClient
{
    // Core
    Task<bool> ConnectAsync();
    Task DisconnectAsync();
    Task<bool> IsConnectedAsync();
    Task<DataValue> ReadValueAsync(string nodeId);
    Task<bool> WriteValueAsync(string nodeId, int value);
    Task<List<DataValue>> ReadMultipleValuesAsync(List<string> nodeIds);
    Task<bool> SubscribeAsync(string nodeId, Action<DataValue> callback);
    Task<bool> UnsubscribeAsync(string nodeId);
    Task<List<ReferenceDescription>> BrowseAsync(string nodeId);
    Task<bool> SubscribeToAllNodesAsync(List<string> nodeIds, Action<string, DataValue> callback);

    Task<bool> ReadBooleanValueAsync(string nodeId);
}
