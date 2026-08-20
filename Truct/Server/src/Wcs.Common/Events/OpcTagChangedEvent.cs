using System.Text.Json.Serialization;

namespace Wcs.Common.Events;

public class OpcTagChangedEvent
{
  public string NodeId { get; set; } = string.Empty;
  public object? OldValue { get; set; }
  public object? NewValue { get; set; }
  public DateTime Timestamp { get; set; }
  public static string EventType => "OpcTagChanged";
  
  public OpcTagChangedEvent() {
    Timestamp = DateTime.UtcNow;
  }
  
  public OpcTagChangedEvent(string nodeId, object? oldValue, object? newValue) {
    NodeId = nodeId;
    OldValue = oldValue;
    NewValue = newValue;
    Timestamp = DateTime.UtcNow;
  }
}
