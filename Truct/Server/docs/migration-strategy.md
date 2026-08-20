# Migration Strategy cho Wcs.Cms

## Tổng quan

Khi tạo project **Wcs.Cms** để quản lý master data, cần quyết định chiến lược quản lý database migrations.

## Kiến trúc hiện tại

- **Wcs.Api**: Chứa `WcsDbContext` và migrations hiện tại
- **Database**: `WcsDb` (SQL Server)
- **Master Data hiện tại**: Robot và Station được load từ config (KHÔNG lưu trong DB)

## Lựa chọn Migration Strategy

### ✅ **Option 1: Centralized Migration Management (KHUYẾN NGHỊ)**

Nếu `Wcs.Cms` và `Wcs.Api` dùng **cùng một database** (`WcsDb`):

**Quản lý migration tập trung ở `Wcs.Api`**

#### Cấu trúc:
```
src/
├── Wcs.Api/
│   ├── Data/
│   │   ├── WcsDbContext.cs          # DbContext chính
│   │   ├── Models/                  # DB Models từ cả Wcs.Api và Wcs.Cms
│   │   └── Configurations/          # Entity configurations
│   └── Migrations/                  # TẤT CẢ migrations ở đây
│
├── Wcs.Cms/
│   ├── Controllers/                 # API controllers
│   ├── Services/                    # Business logic
│   └── Data/
│       └── Models/                  # Master data DB Models
│           ├── RobotDbModel.cs      # Nếu migrate Robot từ config → DB
│           └── StationDbModel.cs    # Nếu migrate Station từ config → DB
│
└── Wcs.Common/
    └── Entities/                    # Domain entities (shared)
```

#### Workflow:

1. **Tạo DB Models trong Wcs.Cms**:
   ```csharp
   // src/Wcs.Cms/Data/Models/RobotDbModel.cs
   [Table("Robots")]
   public class RobotDbModel
   {
       [Key]
       public Guid Id { get; set; }
       public string Code { get; set; } = string.Empty;
       // ... other properties
   }
   ```

2. **Add DbSets vào WcsDbContext (trong Wcs.Api)**:
   ```csharp
   // src/Wcs.Api/Data/WcsDbContext.cs
   public class WcsDbContext : DbContext
   {
       // Existing DbSets
       public DbSet<FlowTaskDbModel> FlowTasks { get; set; }
       
       // Master data DbSets
       public DbSet<RobotDbModel> Robots { get; set; }
       public DbSet<StationDbModel> Stations { get; set; }
   }
   ```

3. **Tạo migration từ Wcs.Api**:
   ```bash
   cd src/Wcs.Api
   dotnet ef migrations add AddMasterDataTables --context WcsDbContext
   dotnet ef database update
   ```

#### Ưu điểm:
- ✅ Tránh conflict migration giữa các projects
- ✅ Dễ quản lý versioning và history
- ✅ Schema nhất quán trong một database
- ✅ Chỉ một nơi để run `dotnet ef` commands

#### Nhược điểm:
- ⚠️ Wcs.Cms phụ thuộc vào Wcs.Api cho DbContext (có thể giải quyết bằng shared DbContext project)

---

### **Option 2: Separate Migration Management**

Nếu `Wcs.Cms` dùng **database riêng** (`WcsCmsDb`):

**Mỗi project quản lý migration riêng**

#### Cấu trúc:
```
src/
├── Wcs.Api/
│   ├── Data/
│   │   └── WcsDbContext.cs
│   └── Migrations/                  # Migrations cho WcsDb
│
└── Wcs.Cms/
    ├── Data/
    │   └── WcsCmsDbContext.cs       # DbContext riêng
    └── Migrations/                  # Migrations cho WcsCmsDb
```

#### Workflow:
```bash
# Migrations cho Wcs.Api
cd src/Wcs.Api
dotnet ef migrations add AddFlowTaskTable --context WcsDbContext

# Migrations cho Wcs.Cms
cd src/Wcs.Cms
dotnet ef migrations add AddRobotsTable --context WcsCmsDbContext
```

---

### **Option 3: Shared Infrastructure Project (NÂNG CAO)**

Tách DbContext và Migrations ra một project riêng:

#### Cấu trúc:
```
src/
├── Wcs.Infrastructure/              # NEW PROJECT
│   ├── Data/
│   │   ├── WcsDbContext.cs
│   │   └── Configurations/
│   └── Migrations/                  # TẤT CẢ migrations
│
├── Wcs.Api/
│   └── (reference Wcs.Infrastructure)
│
└── Wcs.Cms/
    └── (reference Wcs.Infrastructure)
```

#### Ưu điểm:
- ✅ Clean separation of concerns
- ✅ DbContext và migrations độc lập với application projects
- ✅ Dễ mở rộng cho các projects khác

---

## Khuyến nghị cho Wcs.Cms

### **Kịch bản 1: Wcs.Cms quản lý Robot/Station trong DB**

**→ Chọn Option 1: Centralized Migration Management**

Vì:
- Robot/Station có thể được reference từ FlowTasks (foreign key hoặc code reference)
- Cùng database giúp maintain data consistency
- Tránh phức tạp hóa kiến trúc

**Các bước thực hiện:**

1. Tạo DB Models trong `Wcs.Cms/Data/Models/`
2. Add DbSets vào `WcsDbContext` trong `Wcs.Api`
3. Tạo migration từ `Wcs.Api`
4. Update mappers và repositories để support cả config và DB

### **Kịch bản 2: Wcs.Cms quản lý master data độc lập**

**→ Chọn Option 2 hoặc Option 3**

Nếu master data hoàn toàn độc lập và không liên quan đến FlowTasks.

---

## Best Practices

### ✅ DOs
- Luôn backup database trước khi apply migration
- Review migration scripts trước khi apply
- Test migrations trên development environment trước
- Version control tất cả migrations
- Document breaking changes trong migration

### ❌ DON'Ts
- Không tạo migration từ nhiều projects nếu dùng cùng DB
- Không edit migration files đã được apply (tạo migration mới thay vì)
- Không skip migration history
- Không mix manual SQL changes với EF migrations

---

## Migration Commands Reference

```bash
# Tạo migration mới
cd src/Wcs.Api
dotnet ef migrations add MigrationName --context WcsDbContext

# Apply migration
dotnet ef database update --context WcsDbContext

# Rollback migration
dotnet ef database update PreviousMigrationName --context WcsDbContext

# Generate SQL script (không apply)
dotnet ef migrations script --context WcsDbContext --output migration.sql

# List migrations
dotnet ef migrations list --context WcsDbContext

# Remove last migration (chưa apply)
dotnet ef migrations remove --context WcsDbContext
```

---

## Kết luận

**Với use case của bạn (Wcs.Cms quản lý master data):**

→ **Khuyến nghị: Option 1 - Centralized Migration Management**

Quản lý migration tập trung ở `Wcs.Api` vì:
1. Master data thường share với operational data
2. Tránh conflict và complexity
3. Dễ maintain và deploy
4. Phù hợp với kiến trúc hiện tại
