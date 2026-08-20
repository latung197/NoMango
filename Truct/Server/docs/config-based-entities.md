# Config-based Entities Architecture

## Tổng quan

Sau khi tái cấu trúc, **Robot** và **Station** được load từ config thay vì lưu trong database. Điều này phù hợp vì chúng là những giá trị cố định, ít thay đổi và được cấu hình sẵn trong hệ thống.

## Kiến trúc mới

### 🗃️ Database Schema (Simplified)

```
FlowTasks
├── Id (PK)
├── TaskId (Unique)
├── FromStationCode (string) → Config reference
├── ToStationCode (string)   → Config reference  
├── RobotCode (string)       → Config reference
├── CurrentStep
├── RcsTaskId
├── Status
├── CreatedAt/UpdatedAt/CompletedAt
└── Related tables (WaitingSets, History)
```

### 📁 Code Structure

```
src/Wcs.Api/
├── Services/
│   └── ConfigEntityService.cs    # Load Robot/Station từ config
├── Data/
│   ├── Models/
│   │   └── FlowTaskDbModel.cs     # Chỉ lưu codes, không có foreign keys
│   ├── Mappers/
│   │   └── EntityMapper.cs        # Convert với config entities
│   └── Repositories/
│       └── FlowTaskRepository.cs  # Load entities từ config
```

## Lợi ích

### ✅ **Performance**
- Database nhẹ hơn, ít bảng hơn
- Không cần JOIN với Robot/Station tables
- Queries nhanh hơn

### ✅ **Simplicity** 
- Không cần quản lý foreign keys
- Không cần seed data cho Robot/Station
- Schema đơn giản hơn

### ✅ **Configuration Management**
- Robot/Station được quản lý tập trung trong config
- Dễ thay đổi cấu hình mà không cần migrate DB
- Version control cho config files

## Implementation Details

### 1. ConfigEntityService

```csharp
public class ConfigEntityService
{
    public Robot? GetRobotByCode(string code)
    {
        // Load từ config files
        return new Robot(code, $"Robot {code}", true);
    }
    
    public Station? GetStationByCode(string code)
    {
        // Load từ config files  
        return GetStationFromConfig(code);
    }
}
```

### 2. FlowTask DB Model

```csharp
public class FlowTaskDbModel
{
    // Thay vì foreign keys
    public string FromStationCode { get; set; }  // "ST_IN_01"
    public string ToStationCode { get; set; }    // "ST_OUT_01" 
    public string RobotCode { get; set; }        // "ROBOT_001"
    
    // Không có navigation properties
}
```

### 3. Repository Pattern

```csharp
public async Task<FlowTask?> GetByIdAsync(Guid id)
{
    var dbModel = await _context.FlowTasks
        .Include(ft => ft.WaitingSets)
        .FirstOrDefaultAsync(ft => ft.Id == id);

    // Load related entities từ config
    var robot = _configEntityService.GetRobotByCode(dbModel.RobotCode);
    var fromStation = _configEntityService.GetStationByCode(dbModel.FromStationCode);
    var toStation = _configEntityService.GetStationByCode(dbModel.ToStationCode);

    return dbModel.ToDomain(robot, fromStation, toStation);
}
```

## Database Schema Changes

### Before (Foreign Keys)
```sql
CREATE TABLE FlowTasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FromStationId UNIQUEIDENTIFIER NOT NULL,
    ToStationId UNIQUEIDENTIFIER NOT NULL,
    RobotId UNIQUEIDENTIFIER NOT NULL,
    -- Foreign key constraints
    CONSTRAINT FK_FlowTasks_FromStation FOREIGN KEY (FromStationId) REFERENCES Stations(Id)
);
```

### After (Config References)
```sql
CREATE TABLE FlowTasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FromStationCode NVARCHAR(50) NOT NULL,  -- "ST_IN_01"
    ToStationCode NVARCHAR(50) NOT NULL,    -- "ST_OUT_01"
    RobotCode NVARCHAR(50) NOT NULL,        -- "ROBOT_001"
    -- No foreign key constraints
);
```

## Migration Strategy

### 1. Data Migration
```sql
-- Convert existing foreign keys to codes
UPDATE FlowTasks 
SET FromStationCode = (SELECT Code FROM Stations WHERE Id = FromStationId),
    ToStationCode = (SELECT Code FROM Stations WHERE Id = ToStationId),
    RobotCode = (SELECT Code FROM Robots WHERE Id = RobotId);

-- Drop foreign key constraints
ALTER TABLE FlowTasks DROP CONSTRAINT FK_FlowTasks_FromStation;
ALTER TABLE FlowTasks DROP CONSTRAINT FK_FlowTasks_ToStation;
ALTER TABLE FlowTasks DROP CONSTRAINT FK_FlowTasks_Robot;

-- Drop Robot/Station tables
DROP TABLE Stations;
DROP TABLE Robots;
```

### 2. Code Migration
- Update mappers để convert codes thay vì IDs
- Update repository để load entities từ config
- Update services để sử dụng ConfigEntityService

## Configuration Files

### Robot Config (robots.json)
```json
{
  "robots": [
    {
      "code": "ROBOT_001",
      "name": "Robot AGV 001",
      "isActive": true,
      "capabilities": ["pickup", "drop"]
    },
    {
      "code": "ROBOT_002", 
      "name": "Robot AGV 002",
      "isActive": true,
      "capabilities": ["pickup", "drop"]
    }
  ]
}
```

### Station Config (stations.json)
```json
{
  "stations": [
    {
      "code": "ST_IN_01",
      "stageCode": "STAGE_01",
      "type": "IN",
      "hasCurtain": true,
      "curtainStateTag": "PLC.ST_IN_01.CurtainState",
      "curtainControlTag": "PLC.ST_IN_01.CurtainControl"
    },
    {
      "code": "ST_OUT_01",
      "stageCode": "STAGE_01", 
      "type": "OUT",
      "hasCurtain": true,
      "curtainStateTag": "PLC.ST_OUT_01.CurtainState",
      "curtainControlTag": "PLC.ST_OUT_01.CurtainControl"
    }
  ]
}
```

## API Impact

### Before
```csharp
// Cần load với includes
var flowTask = await _context.FlowTasks
    .Include(ft => ft.FromStation)
    .Include(ft => ft.ToStation)
    .Include(ft => ft.Robot)
    .FirstOrDefaultAsync(ft => ft.Id == id);
```

### After
```csharp
// Load từ DB + Config
var dbModel = await _context.FlowTasks
    .Include(ft => ft.WaitingSets)
    .FirstOrDefaultAsync(ft => ft.Id == id);

var robot = _configEntityService.GetRobotByCode(dbModel.RobotCode);
var fromStation = _configEntityService.GetStationByCode(dbModel.FromStationCode);
var toStation = _configEntityService.GetStationByCode(dbModel.ToStationCode);

return dbModel.ToDomain(robot, fromStation, toStation);
```

## Best Practices

### ✅ DOs
- Cache config entities trong memory để tránh load lại
- Validate config entities khi startup
- Sử dụng strongly-typed config classes
- Log khi không tìm thấy entity trong config

### ❌ DON'Ts
- Không hardcode robot/station codes trong business logic
- Không load config entities trong mỗi request
- Không ignore validation của config entities

## Performance Considerations

### Caching Strategy
```csharp
public class ConfigEntityService
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
public async Task<FlowTask> GetFlowTaskWithRobotAsync(Guid id)
{
    var flowTask = await GetByIdAsync(id);
    if (flowTask.Robot == null)
    {
        var robot = _configEntityService.GetRobotByCode(flowTask.RobotCode);
        flowTask.Robot = robot;
    }
    return flowTask;
}
```

## Kết luận

Việc chuyển Robot và Station sang config-based approach giúp:
- **Database đơn giản hơn** - ít bảng, ít foreign keys
- **Performance tốt hơn** - không cần JOIN operations
- **Configuration linh hoạt** - dễ thay đổi mà không cần migrate
- **Maintenance dễ dàng** - config files dễ quản lý hơn database

Đây là một architectural decision phù hợp cho những entities có tính chất cố định và ít thay đổi.
