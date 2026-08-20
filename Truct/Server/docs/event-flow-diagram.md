# Event Flow Diagram

## Luồng xử lý Event End-to-End

```
┌─────────────────────────────────────────────────────────────────┐
│                      INPUT SOURCES                              │
└─────────────────────────────────────────────────────────────────┘
                                │
        ┌───────────────────────┼───────────────────────┐
        │                       │                       │
        ▼                       ▼                       ▼
  ┌──────────┐          ┌──────────┐           ┌──────────┐
  │ OPC UA   │          │ Webhook  │           │ Manual   │
  │ Monitor  │          │ Endpoint │           │ API Call │
  └────┬─────┘          └────┬─────┘           └────┬─────┘
       │                     │                      │
       │ OpcTagChangedEvent  │ WebhookEvent         │ Direct call
       ▼                     ▼                      ▼
┌─────────────────────────────────────────────────────────────────┐
│                    IEventPublisher (Bus)                        │
│                  (InMemoryEventPublisher)                       │
└────────────────────────┬────────────────────────────────────────┘
                         │
                         │ Dispatch to subscribers
                         │
        ┌────────────────┴────────────────┐
        │                                 │
        ▼                                 ▼
┌──────────────────┐           ┌──────────────────┐
│ OpcTagChanged    │           │ Other Event      │
│ EventHandler     │           │ Handlers         │
└────────┬─────────┘           └──────────────────┘
         │
         │ 1. Nhận low-level event
         ▼
┌──────────────────────────────┐
│ EventTransformerRegistry     │
│  - GetTransformers<T>()      │
│  - TransformAsync()          │
└────────┬─────────────────────┘
         │
         │ 2. Tìm transformers
         ▼
┌──────────────────────────────┐
│ OpcTagEventTransformer       │
│  - CanTransform()            │
│  - TransformAsync()          │
└────────┬─────────────────────┘
         │
         │ 3. Phân tích & Transform
         │
         ├─────────┬─────────┬─────────┐
         ▼         ▼         ▼         ▼
  ┌───────────┐ ┌────────┐ ┌────────┐ ┌────────┐
  │ Curtain   │ │Conveyor│ │ Robot  │ │ Other  │
  │ Opened    │ │ Ready  │ │ Events │ │ Events │
  └─────┬─────┘ └────┬───┘ └────┬───┘ └────┬───┘
        │            │          │          │
        └────────────┴──────────┴──────────┘
                     │
                     │ 4. Publish domain events
                     ▼
         ┌───────────────────────┐
         │  IEventPublisher      │
         └───────────┬───────────┘
                     │
        ┌────────────┴────────────┐
        │                         │
        ▼                         ▼
┌──────────────┐          ┌──────────────┐
│   MainFlow   │          │ Domain Event │
│   Handler    │          │   Handlers   │
└──────┬───────┘          └──────────────┘
       │
       │ 5. Process business logic
       │
       ├─ Check waiting requirements
       │
       ├─ Update task state
       │
       └─ Trigger next step
```

## Chi tiết từng bước

### Bước 1: Input Sources

**OPC UA Monitor:**
```csharp
// OpcUaMonitorWorker.cs
private void OnNodeValueChanged(string nodeId, DataValue dataValue)
{
    var @event = new OpcTagChangedEvent(nodeId, oldValue, newValue);
    await _eventPublisher.PublishAsync(@event);
}
```

### Bước 2: Event Handler nhận Low-Level Event

**OpcTagChangedEventHandler:**
```csharp
public async Task HandleAsync(OpcTagChangedEvent @event, CancellationToken ct)
{
    // Transform sang domain events
    var domainEvents = await _transformerRegistry.TransformAsync(@event, ct);
    
    // Publish domain events
    foreach (var domainEvent in domainEvents)
    {
        await _eventPublisher.PublishAsync(domainEvent, ct);
    }
}
```

### Bước 3: Transformer xử lý

**OpcTagEventTransformer:**
```csharp
public async Task<IEnumerable<object>> TransformAsync(OpcTagChangedEvent input, CancellationToken ct)
{
    var station = FindStationByTag(input.NodeId);
    
    if (input.NodeId == station.Tag_CurtainState)
    {
        // Transform → CurtainOpened/CurtainClosed
        return new[] { new CurtainOpened(taskId, stationCode) };
    }
    
    if (input.NodeId == station.Tag_HasCassette)
    {
        // Transform → ConveyorReady
        return new[] { new ConveyorReady(taskId, stationCode) };
    }
    
    return Enumerable.Empty<object>();
}
```

### Bước 4: MainFlow xử lý Domain Events

**MainFlow:**
```csharp
public async Task Handle<T>(T @event, CancellationToken ct)
{
    var task = await _store.LoadAsync(taskId);
    
    // Kiểm tra event có hợp lệ không
    if (!ValidateFlowEvent(task, eventName, @event))
        return;
    
    // Xử lý waiting requirements
    if (task.WaitingFor != null)
    {
        task.WaitingFor.Mark(eventName);
        
        if (task.WaitingFor.IsAllMet)
        {
            await ProceedToNextStep(task, ct);
        }
    }
}
```

## Sequence Diagram

```
OPC UA     OpcMonitor    EventBus    OpcHandler    Transformer    EventBus    MainFlow
  │            │            │            │              │            │           │
  │───Tag─────>│            │            │              │            │           │
  │  Changed   │            │            │              │            │           │
  │            │            │            │              │            │           │
  │            │──Publish──>│            │              │            │           │
  │            │ OpcTag     │            │              │            │           │
  │            │ Changed    │            │              │            │           │
  │            │            │            │              │            │           │
  │            │            │──Dispatch─>│              │            │           │
  │            │            │            │              │            │           │
  │            │            │            │──Transform──>│            │           │
  │            │            │            │              │            │           │
  │            │            │            │              │──Returns───│           │
  │            │            │            │              │ Domain     │           │
  │            │            │            │              │ Events     │           │
  │            │            │            │              │            │           │
  │            │            │            │──Publish────────────────>│           │
  │            │            │            │  Curtain                 │           │
  │            │            │            │  Opened                  │           │
  │            │            │            │                          │           │
  │            │            │            │                          │─Dispatch─>│
  │            │            │            │                          │           │
  │            │            │            │                          │           │
  │            │            │            │                          │<──Process─│
  │            │            │            │                          │           │
```

## Monitoring Points

### 1. Low-Level Event Rate
```
Metric: opc_tag_events_total
Description: Tổng số OPC tag events nhận được
```

### 2. Transform Success Rate
```
Metric: event_transform_success_total / event_transform_total
Description: Tỷ lệ transform thành công
```

### 3. Domain Event Rate
```
Metric: domain_events_published_total
Description: Tổng số domain events được publish
```

### 4. Flow Processing Time
```
Metric: flow_processing_duration_ms
Description: Thời gian xử lý một event trong MainFlow
```

## Error Handling

```
┌──────────────┐
│ Low-Level    │
│ Event        │
└──────┬───────┘
       │
       ▼
   Try-Catch
       │
       ├─ Success ──────> Transform
       │
       └─ Error ────────> Log & Continue
                          (không throw ra ngoài)
```

Mỗi layer có error handling riêng:

1. **Transformer**: Log error, return empty list
2. **Handler**: Log error, không crash worker
3. **MainFlow**: Log error, có thể retry hoặc abort task

