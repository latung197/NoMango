# Event Transform Pattern

## Tổng quan

Event Transform Pattern là kiến trúc để chuyển đổi **low-level events** (OPC UA tags, webhooks) thành **domain events** có ý nghĩa nghiệp vụ.

## Kiến trúc

```
┌─────────────────────┐
│  Low-Level Event    │
│  (OpcTagChanged)    │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ EventTransformer    │
│ - CanTransform()    │
│ - TransformAsync()  │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│  Domain Events      │
│  - CurtainOpened    │
│  - ConveyorReady    │
│  - RobotAtWaiting   │
└─────────────────────┘
```

## Components

### 1. IEventTransformer<TInput>

Interface cơ bản cho tất cả transformers:

```csharp
public interface IEventTransformer<in TInput> where TInput : class
{
    Task<IEnumerable<object>> TransformAsync(TInput input, CancellationToken ct = default);
    bool CanTransform(TInput input);
}
```

### 2. OpcTagEventTransformer

Transform OPC UA tag changes thành domain events:

**Input:** `OpcTagChangedEvent` (NodeId, OldValue, NewValue)

**Output:** Domain events như:
- `CurtainOpened` - khi Tag_CurtainState chuyển từ true → false
- `CurtainClosed` - khi Tag_CurtainState chuyển từ false → true  
- `ConveyorReady` - khi Tag_HasCassette = true

**Logic:**
1. Tìm station dựa vào tag name
2. Phân tích loại tag (HasCassette, CurtainState, etc.)
3. Convert giá trị và tạo domain event tương ứng

### 3. EventTransformerRegistry

Quản lý tất cả transformers và thực hiện transformation:

```csharp
// Đăng ký transformer
registry.Register(opcTagTransformer);

// Transform event
var domainEvents = await registry.TransformAsync(opcTagEvent);
```

## Luồng xử lý Event

```
1. OpcUaMonitorWorker nhận thay đổi từ OPC UA
   └─> Publish OpcTagChangedEvent

2. OpcTagChangedEventHandler nhận event
   └─> Gọi EventTransformerRegistry.TransformAsync()
       └─> OpcTagEventTransformer.TransformAsync()
           └─> Tạo domain events (CurtainOpened, ConveyorReady...)

3. Publish domain events
   └─> MainFlow hoặc các handlers khác xử lý
```

## Ví dụ Transformation

### OPC Tag → Curtain Event

**Input:**
```json
{
  "NodeId": "A_Process.Tag02NB_00042",
  "OldValue": true,
  "NewValue": false
}
```

**Transformation Logic:**
1. Tìm station có `Tag_CurtainState = "A_Process.Tag02NB_00042"` → Station A1
2. Phát hiện `HasCurtain = true`
3. Value thay đổi từ `true → false` = Curtain opened
4. Tìm task đang chờ ở station A1

**Output:**
```csharp
new CurtainOpened(TaskId: "abc-123", StationCode: "A1")
```

### OPC Tag → Conveyor Event

**Input:**
```json
{
  "NodeId": "A_Process.Tag02NB_00041",
  "OldValue": false,
  "NewValue": true
}
```

**Transformation Logic:**
1. Tìm station có `Tag_HasCassette = "A_Process.Tag02NB_00041"` → Station A1
2. Phát hiện `HasConveyor = true`
3. Value = `true` = Có cassette
4. Tìm task đang chờ ở station A1

**Output:**
```csharp
new ConveyorReady(TaskId: "abc-123", StationCode: "A1")
```

## Cách thêm Transformer mới

### Bước 1: Tạo Transformer class

```csharp
public class WebhookEventTransformer : IEventTransformer<WebhookReceivedEvent>
{
    public bool CanTransform(WebhookReceivedEvent input)
    {
        // Logic kiểm tra
        return input.Source == "RCS";
    }

    public async Task<IEnumerable<object>> TransformAsync(WebhookReceivedEvent input, CancellationToken ct)
    {
        var events = new List<object>();
        
        // Logic transform
        if (input.EventType == "robot_arrived")
        {
            events.Add(new RobotAtWaitingPoint(input.TaskId));
        }
        
        return events;
    }
}
```

### Bước 2: Đăng ký trong IoC.cs

```csharp
// Đăng ký service
services.AddSingleton<WebhookEventTransformer>();

// Đăng ký vào registry
private static void RegisterEventTransformers(IServiceCollection services)
{
    var tempServiceProvider = services.BuildServiceProvider();
    var registry = tempServiceProvider.GetRequiredService<EventTransformerRegistry>();
    
    var opcTransformer = tempServiceProvider.GetRequiredService<OpcTagEventTransformer>();
    var webhookTransformer = tempServiceProvider.GetRequiredService<WebhookEventTransformer>();
    
    registry.Register(opcTransformer);
    registry.Register(webhookTransformer);
}
```

### Bước 3: Sử dụng trong Handler

```csharp
public class WebhookEventHandler : IEventHandler<WebhookReceivedEvent>
{
    public async Task HandleAsync(WebhookReceivedEvent @event, CancellationToken ct)
    {
        var domainEvents = await _transformerRegistry.TransformAsync(@event, ct);
        
        foreach (var domainEvent in domainEvents)
        {
            await _eventPublisher.PublishAsync(domainEvent, ct);
        }
    }
}
```

## Lợi ích

✅ **Tách biệt concerns**: Low-level events không ảnh hưởng đến domain logic  
✅ **Dễ test**: Mock transformers để test riêng biệt  
✅ **Mở rộng**: Thêm transformer mới không ảnh hưởng code cũ  
✅ **Logging**: Trace được toàn bộ event transformation  
✅ **Reusable**: Một low-level event có thể tạo nhiều domain events  

## Best Practices

1. **Một transformer chỉ xử lý một loại input event**
2. **CanTransform() phải nhanh và lightweight**
3. **TransformAsync() có thể trả về empty list nếu không transform được**
4. **Log đầy đủ để debug**
5. **Handle errors gracefully - không throw exception ra ngoài**

## Troubleshooting

### Event không được transform?

1. Kiểm tra `CanTransform()` có trả về `true` không
2. Kiểm tra transformer đã được đăng ký trong registry chưa
3. Check logs để xem có exception không

### Domain event không được publish?

1. Kiểm tra `TransformAsync()` có trả về event không
2. Kiểm tra EventPublisher có hoạt động không
3. Kiểm tra có handler nào subscribe domain event đó không

## Tương lai

- [ ] Thêm WebhookEventTransformer
- [ ] Thêm RcsResponseEventTransformer  
- [ ] Thêm persistent event store
- [ ] Thêm event replay mechanism

