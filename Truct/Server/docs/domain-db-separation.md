# Domain Entities và Database Models Separation

## Tổng quan

Dự án đã được tái cấu trúc để tách biệt **Domain Entities** (thuần túy, chứa business logic) khỏi **Database Models** (chỉ cho việc giao tiếp với database). Đây là một pattern quan trọng trong Clean Architecture và Domain-Driven Design.

## Cấu trúc sau khi tái cấu trúc

### 📁 Domain Entities (`src/Wcs.Common/Entities/`)
- **FlowTask**: Entity thuần túy với business logic
- **Robot**: Entity quản lý robot với business methods
- **Station**: Entity quản lý station với validation logic
- **WaitingSet**: Value object cho waiting conditions

**Đặc điểm:**
- ✅ Không có EF annotations
- ✅ Chứa business logic và validation
- ✅ Có thể test độc lập
- ✅ Không phụ thuộc vào database

### 📁 Database Models (`src/Wcs.Api/Data/Models/`)
- **FlowTaskDbModel**: DB model cho FlowTask
- **RobotDbModel**: DB model cho Robot  
- **StationDbModel**: DB model cho Station
- **FlowTaskWaitingSetDbModel**: DB model cho waiting sets
- **FlowTaskHistoryDbModel**: DB model cho history tracking

**Đặc điểm:**
- ✅ Có đầy đủ EF annotations
- ✅ Chỉ focus vào database mapping
- ✅ Virtual properties cho navigation
- ✅ Indexes và constraints

### 📁 Mappers (`src/Wcs.Api/Data/Mappers/`)
- **EntityMapper**: Convert giữa Domain Entities và DB Models

### 📁 Repository (`src/Wcs.Api/Data/Repositories/`)
- **FlowTaskRepository**: Implement interface, convert data từ DB sang Domain

### 📁 DbContext (`src/Wcs.Api/Data/`)
- **WcsDbContext**: EF DbContext làm việc với DB Models

## Luồng dữ liệu

```
Controller
    ↓ (Domain Entities)
Service Layer (Business Logic)
    ↓ (Domain Entities)  
Repository Interface
    ↓ (Domain Entities)
Repository Implementation
    ↓ (Mapper converts Domain ↔ DB Models)
DbContext
    ↓ (DB Models)
Database
```

## Lợi ích của việc tách này

### 🎯 **Separation of Concerns**
- Domain logic tách biệt khỏi database concerns
- Business rules không bị ảnh hưởng bởi thay đổi database
- Dễ test business logic

### 🔧 **Maintainability**
- Thay đổi database schema không ảnh hưởng domain
- Có thể thay đổi ORM mà không ảnh hưởng business logic
- Code rõ ràng, dễ hiểu

### 🧪 **Testability**
- Domain entities có thể test pure unit tests
- Mock repository interfaces dễ dàng
- Không cần database để test business logic

### 🚀 **Performance**
- Có thể optimize DB queries riêng biệt
- Lazy loading chỉ ở DB layer
- Domain objects nhẹ, không có overhead của EF

## Ví dụ sử dụng

### 1. Tạo FlowTask (Business Logic)

```csharp
// Service layer - pure business logic
public async Task<FlowTask> CreateFlowTaskAsync(
    string taskId, 
    Station fromStation, 
    Station toStation, 
    Robot robot)
{
    // Business validation
    if (!await CanCreateFlowTaskAsync(fromStation, toStation, robot))
        throw new BusinessException("Cannot create task");

    // Create domain entity
    var flowTask = new FlowTask(taskId, fromStation, toStation, robot, FlowStep.Initial);
    
    // Repository handles DB conversion
    return await _flowTaskRepository.CreateAsync(flowTask);
}
```

### 2. Repository Implementation (DB Layer)

```csharp
public async Task<FlowTask> CreateAsync(FlowTask flowTask, CancellationToken cancellationToken = default)
{
    // Convert domain to DB model
    var dbModel = flowTask.ToDbModelWithWaitingSets();
    _context.FlowTasks.Add(dbModel);
    
    // Save to database
    await _context.SaveChangesAsync(cancellationToken);
    
    // Return domain entity
    var savedDbModel = await GetDbModelByIdAsync(dbModel.Id, cancellationToken);
    return savedDbModel!.ToDomain();
}
```

### 3. Business Logic trong Domain Entity

```csharp
public class FlowTask
{
    // Business method
    public bool CanMoveTo(FlowStep targetStep)
    {
        return targetStep switch
        {
            FlowStep.EnterPickupPoint => CurrentStep == FlowStep.BeforePick && AreAllWaitingConditionsMet(),
            FlowStep.EnterDropPoint => CurrentStep == FlowStep.BeforeDrop && AreAllWaitingConditionsMet(),
            // ... other validations
        };
    }
    
    // Domain logic
    public void Complete()
    {
        CurrentStep = FlowStep.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

## Migration từ cấu trúc cũ

### Trước (Entity có EF annotations):
```csharp
[Table("FlowTasks")]
public class FlowTask
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("RobotId")]
    public virtual Robot Robot { get; set; }
    
    // Business logic mixed với DB concerns
}
```

### Sau (Tách biệt):
```csharp
// Domain Entity (pure)
public class FlowTask
{
    public Guid Id { get; set; }
    public Robot Robot { get; set; }
    
    // Pure business logic
    public bool CanMoveTo(FlowStep step) { ... }
}

// DB Model (infrastructure)
[Table("FlowTasks")]
public class FlowTaskDbModel
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("RobotId")]
    public virtual RobotDbModel Robot { get; set; }
}
```

## Best Practices

### ✅ DOs
- Luôn convert qua mapper khi làm việc với repository
- Giữ business logic trong domain entities
- Test domain logic độc lập với database
- Sử dụng repository interfaces cho dependency injection

### ❌ DON'Ts
- Không expose DB models ra ngoài repository layer
- Không đặt EF annotations trong domain entities
- Không gọi DbContext trực tiếp từ service layer
- Không mix business logic với DB concerns

## Tools và Commands

### Restore packages
```bash
dotnet restore
```

### Tạo migration (nếu cần)
```bash
cd src/Wcs.Api
dotnet ef migrations add NewMigration
dotnet ef database update
```

### Run project
```bash
cd src/Wcs.Api
dotnet run
```

## File Structure

```
src/
├── Wcs.Common/
│   ├── Entities/              # 🟢 Domain Entities (pure)
│   │   ├── FlowTask.cs
│   │   ├── Robot.cs
│   │   ├── Station.cs
│   │   └── WaitingSet.cs
│   ├── ValueObjects/          # 🟢 Value Objects
│   ├── Abstractions/         
│   │   └── Repositories/      # 🟢 Repository Interfaces
│   └── Services/              # 🟢 Domain Services
│
└── Wcs.Api/
    ├── Data/
    │   ├── Models/            # 🔵 DB Models (EF)
    │   ├── Mappers/           # 🔵 Domain ↔ DB Converters  
    │   ├── Repositories/      # 🔵 Repository Implementations
    │   └── WcsDbContext.cs    # 🔵 EF DbContext
    ├── Controllers/           # 🟡 API Layer
    └── Services/              # 🟡 Application Services
```

## Kết luận

Việc tách biệt Domain Entities và DB Models giúp:
- Code sạch hơn, dễ maintain
- Business logic rõ ràng và testable
- Flexible khi thay đổi database hoặc ORM
- Tuân thủ Clean Architecture principles

Đây là một investment đáng giá cho long-term maintainability của dự án.

