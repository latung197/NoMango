# Hướng dẫn Deploy WCS

## Quy trình deploy code mới

### Cách 1: Sử dụng script tự động (Khuyến nghị)

```bash
# Deploy đầy đủ (bao gồm migration)
./deploy.sh

# Deploy bỏ qua migration (nếu không có thay đổi database)
./deploy.sh --skip-migration
```

### Cách 2: Deploy thủ công từng bước

#### Bước 1: Pull code mới (nếu dùng Git)
```bash
git pull
```

#### Bước 2: Chạy Database Migration (nếu có thay đổi database)
```bash
docker compose run --rm wcs-migrator
```

**Lưu ý:** Chỉ cần chạy migration khi:
- Có migration mới trong `src/Wcs.Infrastructure/Migrations/`
- Database schema đã thay đổi
- Nếu không có thay đổi database, có thể bỏ qua bước này

#### Bước 3: Rebuild Docker Images
```bash
# Rebuild tất cả services
docker compose build

# Hoặc rebuild với --no-cache để đảm bảo build mới hoàn toàn
docker compose build --no-cache

# Hoặc rebuild chỉ một service cụ thể
docker compose build wcs-api
docker compose build wcs-cms
```

#### Bước 4: Restart Containers
```bash
# Dừng containers cũ và khởi động lại với image mới
docker compose down
docker compose up -d

# Hoặc restart nhanh (không rebuild)
docker compose restart
```

#### Bước 5: Kiểm tra trạng thái
```bash
# Xem trạng thái các containers
docker compose ps

# Xem logs để kiểm tra lỗi
docker compose logs -f wcs-api
docker compose logs -f wcs-cms
```

## Các lệnh hữu ích khác

### Xem logs real-time
```bash
# Logs của tất cả services
docker compose logs -f

# Logs của một service cụ thể
docker compose logs -f wcs-api
docker compose logs -f wcs-cms
```

### Kiểm tra trạng thái
```bash
# Danh sách containers
docker compose ps

# Thông tin chi tiết
docker compose ps -a
```

### Dừng/Start services
```bash
# Dừng tất cả
docker compose stop

# Start tất cả
docker compose start

# Restart một service cụ thể
docker compose restart wcs-api
```

### Xóa và tạo lại từ đầu
```bash
# Xóa containers, networks (giữ volumes và images)
docker compose down

# Xóa tất cả bao gồm volumes
docker compose down -v

# Xóa tất cả và rebuild
docker compose down
docker compose build --no-cache
docker compose up -d
```

### Chạy migration riêng
```bash
# Chạy migration
docker compose run --rm wcs-migrator

# Xem danh sách migrations
docker compose run --rm wcs-migrator dotnet ef migrations list --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj --startup-project src/Wcs.Api/Wcs.Api.csproj
```

## Checklist khi deploy

- [ ] Pull code mới từ repository
- [ ] Kiểm tra có migration mới không
- [ ] Chạy migration (nếu có)
- [ ] Rebuild Docker images
- [ ] Restart containers
- [ ] Kiểm tra logs để đảm bảo không có lỗi
- [ ] Test API endpoints
- [ ] Kiểm tra kết nối database
- [ ] Kiểm tra kết nối OPC UA (nếu cần)
- [ ] Kiểm tra kết nối RCS (nếu cần)

## Troubleshooting

### Container không start được
```bash
# Xem logs chi tiết
docker compose logs wcs-api

# Kiểm tra cấu hình
docker compose config
```

### Migration lỗi
```bash
# Kiểm tra connection string
docker compose run --rm wcs-migrator env | grep ConnectionStrings

# Chạy migration với verbose
docker compose run --rm wcs-migrator bash -c "dotnet ef database update --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj --startup-project src/Wcs.Api/Wcs.Api.csproj --verbose"
```

### Image không rebuild
```bash
# Xóa image cũ và rebuild
docker compose down
docker rmi wcs-wcs-api wcs-wcs-cms
docker compose build --no-cache
docker compose up -d
```

## Lưu ý quan trọng

1. **Backup database** trước khi chạy migration trong production
2. **Kiểm tra connection strings** trong docker-compose.yml
3. **Đảm bảo config files** trong thư mục `./config` đã được cập nhật
4. **Kiểm tra ports** không bị conflict (5000, 5001)
5. **Kiểm tra volumes** mount đúng đường dẫn

