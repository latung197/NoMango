#!/bin/bash

# Script deploy cho WCS project
# Sử dụng: ./deploy.sh [--skip-migration]

set -e  # Exit on error

SKIP_MIGRATION=false

# Parse arguments
if [[ "$1" == "--skip-migration" ]]; then
    SKIP_MIGRATION=true
fi

echo "🚀 Bắt đầu quy trình deploy WCS..."

# 1. Pull code mới (nếu dùng git)
if [ -d ".git" ]; then
    echo "📥 Pulling code mới từ git..."
    git pull
else
    echo "⚠️  Không phát hiện git repository, bỏ qua bước pull code"
fi

# 2. Chạy migration (nếu có thay đổi database)
if [ "$SKIP_MIGRATION" = false ]; then
    echo "🗄️  Chạy database migration..."
    docker compose run --rm wcs-migrator
else
    echo "⏭️  Bỏ qua migration (--skip-migration được chỉ định)"
fi

# 3. Rebuild images (với --no-cache để đảm bảo build mới hoàn toàn)
echo "🔨 Rebuilding Docker images..."
docker compose build --no-cache

# 4. Stop và remove containers cũ
echo "🛑 Dừng containers cũ..."
docker compose down

# 5. Start containers mới
echo "▶️  Khởi động containers mới..."
docker compose up -d

# 6. Kiểm tra trạng thái
echo "📊 Kiểm tra trạng thái containers..."
docker compose ps

echo "✅ Deploy hoàn tất!"
echo ""
echo "📝 Logs có thể xem bằng:"
echo "   docker compose logs -f wcs-api"
echo "   docker compose logs -f wcs-cms"

