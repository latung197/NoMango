# Kế hoạch Tạo Wcs.Infrastructure Project

## 📋 Tổng quan

Tạo project `Wcs.Infrastructure` để tập trung quản lý:
- **DbContext** (WcsDbContext)
- **DB Models** (FlowTaskDbModel, FlowTaskWaitingSetDbModel, FlowTaskHistoryDbModel)
- **Migrations** (EF Core migrations)
- **Entity Configurations** (nếu có)

## 🎯 Mục tiêu

- ✅ Tách biệt Infrastructure layer khỏi Application layer
- ✅ Cho phép Wcs.Api và Wcs.Cms dùng chung DbContext và Models
- ✅ Quản lý migrations tập trung ở một nơi
- ✅ Tuân thủ Clean Architecture principles

## 📐 Cấu trúc hiện tại

```
src/Wcs.Api/
├── Data/
│   ├── WcsDbContext.cs                    ← DI CHUYỂN
│   ├── Models/                            ← DI CHUYỂN
│   │   ├── FlowTaskDbModel.cs
│   │   ├── FlowTaskWaitingSetDbModel.cs
│   │   └── FlowTaskHistoryDbModel.cs
│   ├── Mappers/                           ← GIỮ LẠI (có thể di chuyển sau)
│   │   └── EntityMapper.cs
│   └── Repositories/                      ← GIỮ LẠI
│       └── FlowTaskRepository.cs
└── Migrations/                            ← DI CHUYỂN
    ├── 20250929045015_InitialCreate.cs
    └── WcsDbContextModelSnapshot.cs
```

## 🏗️ Cấu trúc sau khi refactor

```
src/
├── Wcs.Infrastructure/                    ← MỚI TẠO
│   ├── Data/
│   │   ├── WcsDbContext.cs               ← Từ Api
│   │   └── Models/                       ← Từ Api
│   │       ├── FlowTaskDbModel.cs
│   │       ├── FlowTaskWaitingSetDbModel.cs
│   │       └── FlowTaskHistoryDbModel.cs
│   └── Migrations/                       ← Từ Api
│       ├── 20250929045015_InitialCreate.cs
│       └── WcsDbContextModelSnapshot.cs
│
├── Wcs.Api/
│   ├── Data/
│   │   ├── Mappers/                      ← GIỮ LẠI (reference Infrastructure)
│   │   └── Repositories/                 ← GIỮ LẠI (reference Infrastructure)
│   └── IoC.cs                            ← UPDATE namespace
│
└── Wcs.Cms/
    └── (sẽ reference Wcs.Infrastructure)
```

## 📝 Các bước thực hiện

### Bước 1: Tạo project Wcs.Infrastructure

```bash
cd src
dotnet new classlib -n Wcs.Infrastructure -f net8.0
cd ..
dotnet sln WCS.sln add src/Wcs.Infrastructure/Wcs.Infrastructure.csproj
```

### Bước 2: Add packages vào Infrastructure

```bash
cd src/Wcs.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add reference ../Wcs.Common/Wcs.Common.csproj
```

### Bước 3: Di chuyển files

#### 3.1. Di chuyển WcsDbContext
- **Từ**: `src/Wcs.Api/Data/WcsDbContext.cs`
- **Đến**: `src/Wcs.Infrastructure/Data/WcsDbContext.cs`
- **Thay đổi namespace**: `Wcs.Api.Data` → `Wcs.Infrastructure.Data`

#### 3.2. Di chuyển DB Models
- **Từ**: `src/Wcs.Api/Data/Models/*.cs`
- **Đến**: `src/Wcs.Infrastructure/Data/Models/*.cs`
- **Thay đổi namespace**: `Wcs.Api.Data.Models` → `Wcs.Infrastructure.Data.Models`

#### 3.3. Di chuyển Migrations
- **Từ**: `src/Wcs.Api/Migrations/*.cs`
- **Đến**: `src/Wcs.Infrastructure/Migrations/*.cs`
- **Update namespace trong migration files**: `Wcs.Api.Migrations` → `Wcs.Infrastructure.Migrations`
- **Update DbContext reference**: `Wcs.Api.Data.WcsDbContext` → `Wcs.Infrastructure.Data.WcsDbContext`

### Bước 4: Update references trong Wcs.Api

#### 4.1. Update project reference
```bash
cd src/Wcs.Api
dotnet add reference ../Wcs.Infrastructure/Wcs.Infrastructure.csproj
```

#### 4.2. Update using statements

**FlowTaskRepository.cs**:
```csharp
// Trước
using Wcs.Api.Data;
using Wcs.Api.Data.Models;

// Sau
using Wcs.Infrastructure.Data;
using Wcs.Infrastructure.Data.Models;
```

**EntityMapper.cs**:
```csharp
// Trước
using Wcs.Api.Data.Models;

// Sau
using Wcs.Infrastructure.Data.Models;
```

**IoC.cs**:
```csharp
// Trước
using Wcs.Api.Data;

// Sau
using Wcs.Infrastructure.Data;
```

#### 4.3. Update Migration commands

Trong `.csproj` hoặc khi chạy commands:
```bash
# Trước
cd src/Wcs.Api
dotnet ef migrations add MigrationName

# Sau
cd src/Wcs.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Wcs.Api
```

### Bước 5: Cấu hình Design-time DbContext Factory

Tạo `WcsDbContextFactory.cs` trong Infrastructure để EF Tools có thể tạo migrations:

```csharp
// src/Wcs.Infrastructure/Data/WcsDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wcs.Infrastructure.Data;

public class WcsDbContextFactory : IDesignTimeDbContextFactory<WcsDbContext>
{
    public WcsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WcsDbContext>();
        
        // Sử dụng connection string mặc định cho design-time
        // Hoặc đọc từ environment variable
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=tcp:127.0.0.1,1433;Database=WcsDb;User Id=sa;Password=secret;Encrypt=false";
            
        optionsBuilder.UseSqlServer(connectionString);
        
        return new WcsDbContext(optionsBuilder.Options);
    }
}
```

### Bước 6: Test và Verify

1. **Build solution**:
```bash
dotnet build
```

2. **Verify migrations**:
```bash
cd src/Wcs.Infrastructure
dotnet ef migrations list --startup-project ../Wcs.Api
```

3. **Test database connection**:
```bash
dotnet ef database update --startup-project ../Wcs.Api
```

## ⚠️ Lưu ý quan trọng

### 1. Migration Namespace
Sau khi di chuyển migrations, cần update namespace trong các file migration:
- `Wcs.Api.Migrations` → `Wcs.Infrastructure.Migrations`
- `using Wcs.Api.Data;` → `using Wcs.Infrastructure.Data;`

### 2. DbContextModelSnapshot
File `WcsDbContextModelSnapshot.cs` cũng cần update namespace và reference.

### 3. Connection String
Design-time factory cần có cách lấy connection string. Có thể:
- Dùng environment variable
- Tạo `appsettings.json` trong Infrastructure (cho design-time)
- Hoặc hard-code cho development (không khuyến nghị cho production)

### 4. Repository Dependencies
Repositories trong `Wcs.Api` sẽ vẫn phụ thuộc vào `WcsDbContext` từ Infrastructure, điều này OK vì đây là dependency đúng hướng (Application → Infrastructure).

## 🔄 Migration Commands sau khi refactor

```bash
# Tạo migration mới
cd src/Wcs.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Wcs.Api

# Apply migration
cd src/Wcs.Infrastructure
dotnet ef database update --startup-project ../Wcs.Api

# Generate SQL script
cd src/Wcs.Infrastructure
dotnet ef migrations script --startup-project ../Wcs.Api --output migration.sql

# List migrations
cd src/Wcs.Infrastructure
dotnet ef migrations list --startup-project ../Wcs.Api
```

## ✅ Checklist

- [ ] Tạo project Wcs.Infrastructure
- [ ] Add packages (EF Core, EF Tools)
- [ ] Add reference đến Wcs.Common
- [ ] Di chuyển WcsDbContext
- [ ] Di chuyển DB Models (3 files)
- [ ] Di chuyển Migrations
- [ ] Update namespaces trong migrated files
- [ ] Update Wcs.Api project reference
- [ ] Update using statements trong Wcs.Api
- [ ] Tạo Design-time DbContext Factory
- [ ] Update IoC.cs
- [ ] Build và test
- [ ] Verify migrations work

## 🎯 Kết quả mong đợi

Sau khi hoàn thành:
- ✅ `Wcs.Api` và `Wcs.Cms` có thể dùng chung DbContext và Models
- ✅ Migrations được quản lý tập trung ở Infrastructure
- ✅ Code tuân thủ Clean Architecture
- ✅ Dễ mở rộng và maintain

## 📚 Reference

- [EF Core Design-time Services](https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
