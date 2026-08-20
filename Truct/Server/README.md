# WCS - Warehouse Control System

Hệ thống quản lý và điều khiển kho tự động, tích hợp với Robot Control System (RCS) và OPC UA để quản lý luồng công việc và điều khiển thiết bị.

## 📋 Mục lục

- [Tổng quan](#tổng-quan)
- [Kiến trúc](#kiến-trúc)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Cài đặt](#cài-đặt)
- [Cấu hình](#cấu-hình)
- [Chạy ứng dụng](#chạy-ứng-dụng)
- [Database Migration](#database-migration)
- [Build](#build)
- [Deploy](#deploy)
- [Cấu trúc Project](#cấu-trúc-project)
- [Scripts hữu ích](#scripts-hữu-ích)
- [Troubleshooting](#troubleshooting)

## 🎯 Tổng quan

WCS là hệ thống điều khiển kho tự động với các tính năng chính:

- **Quản lý Flow Tasks**: Điều phối các nhiệm vụ di chuyển trong kho
- **Tích hợp RCS**: Giao tiếp với Robot Control System để điều khiển robot
- **Tích hợp OPC UA**: Đọc/ghi dữ liệu từ PLC qua OPC UA
- **Quản lý Master Data**: Quản lý Station, Robot, Flow, Stage, Cassette
- **Event-driven Architecture**: Sử dụng CAP (DotNetCore.CAP) cho messaging
- **Real-time Monitoring**: Theo dõi trạng thái thiết bị và task real-time

## 🏗️ Kiến trúc

### Project Structure

```
Wcs/
├── src/
│   ├── Wcs.Api/              # Main API service (Flow Task Management)
│   ├── Wcs.Cms/              # CMS API service (Master Data Management)
│   ├── Wcs.Common/            # Shared domain entities và abstractions
│   ├── Wcs.Infrastructure/   # Database, Repositories, Migrations
│   ├── Wcs.OpcUa/            # OPC UA client integration
│   └── Wcs.Rcs/              # RCS client integration
├── config/                    # Configuration files (Robot, Station, OPC)
├── docs/                      # Documentation
└── docker-compose.yml         # Docker orchestration
```

### Kiến trúc Clean Architecture

- **Domain Layer** (`Wcs.Common`): Entities, Value Objects, Domain Events
- **Application Layer** (`Wcs.Api`, `Wcs.Cms`): Services, Controllers, Event Handlers
- **Infrastructure Layer** (`Wcs.Infrastructure`): Database, Repositories, External Services

### Technology Stack

- **.NET 8.0**: Framework chính
- **Entity Framework Core 9.0**: ORM
- **SQL Server**: Database
- **DotNetCore.CAP**: Event bus (RabbitMQ)
- **OPC UA**: Industrial communication protocol
- **Docker**: Containerization
- **SignalR**: Real-time notifications (CMS)

## 💻 Yêu cầu hệ thống

### Development

- .NET 8.0 SDK
- SQL Server (hoặc SQL Server Express)
- Docker & Docker Compose (optional)
- Git

### Production

- .NET 8.0 Runtime
- SQL Server
- RabbitMQ (cho CAP)
- Docker & Docker Compose

## 🚀 Cài đặt

### 1. Clone repository

```bash
git clone <repository-url>
cd Wcs
```

### 2. Restore dependencies

```bash
dotnet restore
```

### 3. Cấu hình Database

Cập nhật connection string trong:
- `src/Wcs.Api/appsettings.json`
- `src/Wcs.Cms/appsettings.json`
- `docker-compose.yml` (nếu dùng Docker)

### 4. Chạy Migration

Xem phần [Database Migration](#database-migration)

## ⚙️ Cấu hình

### Configuration Files

Các file cấu hình nằm trong thư mục `config/`:

- `robot.json`: Cấu hình Robot
- `station.json`: Cấu hình Station
- `opc.json`: Cấu hình OPC UA tags
- `signal.json`: Cấu hình tất cả OPC UA tags

### Environment Variables

Các biến môi trường quan trọng (có thể set trong `docker-compose.yml`):

```yaml
ConnectionStrings__DefaultConnection: Server=...;Database=WcsDb;...
ConnectionStrings__CapConnection: Server=...;Database=CapWcsDb;...
ConnectionStrings__RabbitConnection: amqp://user:pass@host:5672/
Rcs__BaseUrl: http://...
OpcUa__EndpointUrl: opc.tcp://...
```

## 🏃 Chạy ứng dụng

### Development (Local)

#### Chạy Wcs.Api

```bash
dotnet watch --project .\src\Wcs.Api
```

API sẽ chạy tại: `http://localhost:5000`

#### Chạy Wcs.Cms

```bash
dotnet watch --project .\src\Wcs.Cms
```

CMS sẽ chạy tại: `http://localhost:5001`

### Docker Compose

```bash
# Build và start tất cả services
docker compose up -d

# Xem logs
docker compose logs -f

# Stop services
docker compose down
```

### Services

- **Wcs.Api**: Port `5000` → Container port `8080`
- **Wcs.Cms**: Port `5001` → Container port `8090`

## 🗄️ Database Migration

### Tạo Migration mới

#### Windows PowerShell:
```powershell
.\migrate.ps1 [tên-migration]
```

#### Linux/Mac/Bash:
```bash
./migrate.sh [tên-migration]
```

#### Hoặc chạy trực tiếp:
```bash
dotnet ef migrations add MigrationName \
  --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj \
  --startup-project src/Wcs.Api/Wcs.Api.csproj
```

### Apply Migration vào Database

#### Windows PowerShell:
```powershell
.\migrate-update.ps1
```

#### Linux/Mac/Bash:
```bash
./migrate-update.sh
```

#### Hoặc chạy trực tiếp:
```bash
dotnet ef database update \
  --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj \
  --startup-project src/Wcs.Api/Wcs.Api.csproj
```

### Chạy Migration trong Docker

```bash
docker compose run --rm wcs-migrator
```

## 📦 Build

Publish .NET Core cho IIS
```bash
dotnet publish -c Release -o .\bin\publish
```

Tạo file transform chỉ định khi publish:
```
Project/
├── appsettings.json
├── Program.cs
└── web.config          # Transform cho publish profile
```

```bash
dotnet publish -c Release -p:IsWebConfigTransformDisabled=true -o .\bin\publish
```

## 🚢 Deploy

### Sử dụng Script tự động

#### Linux/Mac:
```bash
# Deploy đầy đủ (bao gồm migration)
./deploy.sh

# Deploy bỏ qua migration
./deploy.sh --skip-migration
```

### Deploy thủ công

1. **Pull code mới**
   ```bash
   git pull
   ```

2. **Chạy Migration** (nếu có thay đổi database)
   ```bash
   docker compose run --rm wcs-migrator
   ```

3. **Rebuild Images**
   ```bash
   docker compose build --no-cache
   ```

4. **Restart Containers**
   ```bash
   docker compose down
   docker compose up -d
   ```

5. **Kiểm tra**
   ```bash
   docker compose ps
   docker compose logs -f wcs-api
   ```

Xem chi tiết trong [DEPLOY.md](./DEPLOY.md)

## 📁 Cấu trúc Project

### Wcs.Api
- **Controllers**: API endpoints cho Flow Task
- **Services**: Business logic (RobotService, StationService)
- **Workers**: Background workers (OpcUaMonitorWorker, FlowTaskTimeoutWorker)
- **EventHandlers**: Xử lý domain events
- **RobotFlow**: Flow orchestration logic

### Wcs.Cms
- **Controllers**: API endpoints cho Master Data
- **Services**: Business logic cho CMS
- **EventHandlers**: Xử lý events từ API
- **NotificationsHub**: SignalR hub cho real-time updates

### Wcs.Common
- **Entities**: Domain entities (FlowTask, Robot, Station, etc.)
- **ValueObjects**: Value objects (StationTag, StageArea, etc.)
- **Events**: Domain events
- **Abstractions**: Interfaces cho repositories và services
- **Exceptions**: Custom exceptions

### Wcs.Infrastructure
- **Data**: DbContext, DB Models, Mappers
- **Repositories**: Repository implementations
- **Migrations**: EF Core migrations

### Wcs.OpcUa
- OPC UA client implementation
- Connection management
- Tag reading/writing

### Wcs.Rcs
- RCS client implementation
- API communication với RCS system

## 🛠️ Scripts hữu ích

### Migration Scripts

| Script | Mô tả |
|--------|-------|
| `migrate.ps1` / `migrate.sh` | Tạo migration mới |
| `migrate-update.ps1` / `migrate-update.sh` | Apply migration vào database |

### Deploy Scripts

| Script | Mô tả |
|--------|-------|
| `deploy.sh` | Deploy tự động (Linux/Mac) |

### Docker Commands

```bash
# Xem logs
docker compose logs -f [service-name]

# Restart service
docker compose restart [service-name]

# Rebuild một service
docker compose build [service-name]

# Xem trạng thái
docker compose ps
```

## 🔧 Troubleshooting

### Migration lỗi

**Lỗi**: "Pending model changes"

**Giải pháp**:
```bash
# Tạo migration mới
.\migrate.ps1 PendingModelChanges

# Sau đó apply
.\migrate-update.ps1
```

### Container không start

```bash
# Xem logs chi tiết
docker compose logs wcs-api

# Kiểm tra cấu hình
docker compose config
```

### Database connection lỗi

- Kiểm tra connection string trong `docker-compose.yml`
- Đảm bảo SQL Server đang chạy và accessible
- Kiểm tra firewall rules

### OPC UA connection lỗi

- Kiểm tra `OpcUa__EndpointUrl` trong `docker-compose.yml`
- Đảm bảo OPC UA server đang chạy
- Kiểm tra network connectivity

## 📚 Documentation

Xem thêm documentation trong thư mục `docs/`:

- [Domain-DB Separation](./docs/domain-db-separation.md)
- [Infrastructure Refactoring Plan](./docs/infrastructure-refactoring-plan.md)
- [Event Transform Pattern](./docs/event-transform-pattern.md)
- [Migration Strategy](./docs/migration-strategy.md)
- [Service Separation](./docs/service-separation.md)

## 🤝 Contributing

1. Tạo branch mới từ `main`
2. Commit changes với message rõ ràng
3. Tạo Pull Request
4. Đảm bảo code đã được test

## 📝 License

[Thêm license information nếu có]

## 👥 Team

[Thêm team information nếu cần]

---

**Lưu ý**: Đảm bảo đọc kỹ [DEPLOY.md](./DEPLOY.md) trước khi deploy lên production.

