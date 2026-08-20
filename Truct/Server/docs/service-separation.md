# Service Separation Architecture

## Tổng quan

Đã tách `ConfigEntityService` thành `RobotService` và `StationService` để có **Separation of Concerns** rõ ràng hơn và dễ maintain.

## Kiến trúc mới

### 🏗️ Service Layer Structure

```
src/Wcs.Api/Services/
├── RobotService.cs          # Quản lý Robot entities
├── StationService.cs        # Quản lý Station entities
└── FlowTaskService.cs       # Business logic cho FlowTask
```

### 📋 Service Responsibilities

#### RobotService
- ✅ Load Robot từ config
- ✅ Validate Robot capabilities
- ✅ Check Robot status
- ✅ Get Robot information

#### StationService  
- ✅ Load Station từ config
- ✅ Validate Station operations
- ✅ Check Station compatibility
- ✅ Get Station information

## Implementation Details

### 1. RobotService

```csharp
public class RobotService
{
    public Robot? GetRobotByCode(string code)
    public IEnumerable<Robot> GetAllRobots()
    public IEnumerable<Robot> GetActiveRobots()
    public bool IsRobotActive(string code)
    public bool CanRobotHandleTask(string robotCode, string taskType)
    public RobotInfo? GetRobotInfo(string code)
}
```

**Features:**
- Load robots từ config files
- Validate robot capabilities
- Check robot status
- Provide robot information DTOs

### 2. StationService

```csharp
public class StationService
{
    public Station? GetStationByCode(string code)
    public IEnumerable<Station> GetAllStations()
    public IEnumerable<Station> GetStationsByType(InOut type)
    public IEnumerable<Station> GetStationsByStage(string stageCode)
    public bool IsStationActive(string code)
    public bool CanStationAcceptPickup(string code)
    public bool CanStationAcceptDrop(string code)
    public bool CanStationHandleOperation(string stationCode, string operation)
    public StationInfo? GetStationInfo(string code)
}
```

**Features:**
- Load stations từ config files
- Filter stations by type/stage
- Validate station operations
- Check station compatibility
- Provide station information DTOs

## Repository Integration

### Before (ConfigEntityService)
```csharp
public class FlowTaskRepository
{
    private readonly ConfigEntityService _configEntityService;
    
    public async Task<FlowTask?> GetByIdAsync(Guid id)
    {
        var robot = _configEntityService.GetRobotByCode(dbModel.RobotCode);
        var fromStation = _configEntityService.GetStationByCode(dbModel.FromStationCode);
        var toStation = _configEntityService.GetStationByCode(dbModel.ToStationCode);
    }
}
```

### After (Separated Services)
```csharp
public class FlowTaskRepository
{
    private readonly RobotService _robotService;
    private readonly StationService _stationService;
    
    public async Task<FlowTask?> GetByIdAsync(Guid id)
    {
        var robot = _robotService.GetRobotByCode(dbModel.RobotCode);
        var fromStation = _stationService.GetStationByCode(dbModel.FromStationCode);
        var toStation = _stationService.GetStationByCode(dbModel.ToStationCode);
    }
}
```

## Dependency Injection

### IoC Registration
```csharp
// Services
services.AddScoped<FlowTaskService>();
services.AddScoped<RobotService>();      // ✅ New
services.AddScoped<StationService>();    // ✅ New
// services.AddScoped<ConfigEntityService>(); // ❌ Removed
```

## Lợi ích của việc tách

### ✅ **Single Responsibility Principle**
- Mỗi service có một responsibility rõ ràng
- RobotService chỉ quản lý Robot
- StationService chỉ quản lý Station

### ✅ **Easier Testing**
- Test RobotService độc lập
- Test StationService độc lập
- Mock services dễ dàng hơn

### ✅ **Better Maintainability**
- Thay đổi Robot logic không ảnh hưởng Station
- Code organization rõ ràng hơn
- Dễ debug và troubleshoot

### ✅ **Flexible Dependencies**
- Repository có thể inject chỉ RobotService hoặc StationService
- Services có thể có dependencies khác nhau
- Loose coupling giữa các services

## Usage Examples

### 1. Robot Operations
```csharp
public class RobotController
{
    private readonly RobotService _robotService;
    
    [HttpGet("{code}")]
    public ActionResult<RobotInfo> GetRobot(string code)
    {
        var robotInfo = _robotService.GetRobotInfo(code);
        return robotInfo != null ? Ok(robotInfo) : NotFound();
    }
    
    [HttpGet("active")]
    public ActionResult<IEnumerable<Robot>> GetActiveRobots()
    {
        return Ok(_robotService.GetActiveRobots());
    }
}
```

### 2. Station Operations
```csharp
public class StationController
{
    private readonly StationService _stationService;
    
    [HttpGet("{code}")]
    public ActionResult<StationInfo> GetStation(string code)
    {
        var stationInfo = _stationService.GetStationInfo(code);
        return stationInfo != null ? Ok(stationInfo) : NotFound();
    }
    
    [HttpGet("by-type/{type}")]
    public ActionResult<IEnumerable<Station>> GetStationsByType(InOut type)
    {
        return Ok(_stationService.GetStationsByType(type));
    }
}
```

### 3. Business Logic Integration
```csharp
public class FlowTaskService
{
    private readonly RobotService _robotService;
    private readonly StationService _stationService;
    
    public async Task<bool> CanCreateFlowTaskAsync(
        string fromStationCode, 
        string toStationCode, 
        string robotCode)
    {
        // Validate robot
        if (!_robotService.IsRobotActive(robotCode))
            return false;
            
        // Validate stations
        if (!_stationService.CanStationAcceptPickup(fromStationCode))
            return false;
            
        if (!_stationService.CanStationAcceptDrop(toStationCode))
            return false;
            
        return true;
    }
}
```

## Configuration Files Structure

### robots.json
```json
{
  "robots": [
    {
      "code": "ROBOT_001",
      "name": "Robot AGV 001",
      "isActive": true,
      "capabilities": ["pickup", "drop", "transport"],
      "maxLoad": 50,
      "batteryLevel": 85
    },
    {
      "code": "ROBOT_002",
      "name": "Robot AGV 002", 
      "isActive": true,
      "capabilities": ["pickup", "drop"],
      "maxLoad": 30,
      "batteryLevel": 92
    }
  ]
}
```

### stations.json
```json
{
  "stations": [
    {
      "code": "ST_IN_01",
      "stageCode": "STAGE_01",
      "type": "IN",
      "hasCurtain": true,
      "hasConveyor": true,
      "isActive": true,
      "tags": {
        "curtainState": "PLC.ST_IN_01.CurtainState",
        "curtainControl": "PLC.ST_IN_01.CurtainControl",
        "conveyorStage": "PLC.ST_IN_01.ConveyorStage",
        "conveyorProcess": "PLC.ST_IN_01.ConveyorProcess"
      }
    }
  ]
}
```

## Performance Considerations

### Caching Strategy
```csharp
public class RobotService
{
    private readonly IMemoryCache _cache;
    
    public Robot? GetRobotByCode(string code)
    {
        return _cache.GetOrCreate($"robot_{code}", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return LoadRobotFromConfig(code);
        });
    }
}
```

### Lazy Loading
```csharp
// Chỉ load khi cần thiết
public async Task<FlowTask> GetFlowTaskWithDetailsAsync(Guid id)
{
    var flowTask = await GetByIdAsync(id);
    
    // Load additional details only if needed
    if (flowTask.Robot == null)
        flowTask.Robot = _robotService.GetRobotByCode(flowTask.RobotCode);
        
    return flowTask;
}
```

## Migration Strategy

### 1. Code Changes
- ✅ Tạo RobotService và StationService
- ✅ Cập nhật Repository dependencies
- ✅ Cập nhật IoC registration
- ✅ Xóa ConfigEntityService

### 2. Testing
```csharp
[Test]
public void RobotService_GetRobotByCode_ReturnsCorrectRobot()
{
    // Arrange
    var mockConfigService = new Mock<IConfigService>();
    var robotService = new RobotService(mockConfigService.Object, logger);
    
    // Act
    var robot = robotService.GetRobotByCode("ROBOT_001");
    
    // Assert
    Assert.That(robot.Code, Is.EqualTo("ROBOT_001"));
}
```

## Best Practices

### ✅ DOs
- Sử dụng specific services cho specific entities
- Implement proper error handling trong services
- Cache config data để tránh load lại
- Validate input parameters
- Log errors và warnings

### ❌ DON'Ts
- Không mix Robot và Station logic trong cùng service
- Không hardcode config values trong services
- Không ignore error handling
- Không load config data trong mỗi request

## Kết luận

Việc tách `ConfigEntityService` thành `RobotService` và `StationService` giúp:

- **🎯 Clear Separation**: Mỗi service có responsibility rõ ràng
- **🧪 Better Testing**: Dễ test và mock từng service
- **🔧 Easier Maintenance**: Thay đổi logic không ảnh hưởng lẫn nhau
- **📈 Scalability**: Có thể extend services độc lập
- **🏗️ Clean Architecture**: Tuân thủ SOLID principles

Đây là một architectural improvement quan trọng cho long-term maintainability của hệ thống.
