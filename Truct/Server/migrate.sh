#!/bin/bash

# Script để chạy migration từ root directory
# Sử dụng: ./migrate.sh [migration-name]

MIGRATION_NAME=${1:-PendingModelChanges}

echo "🔄 Tạo migration: $MIGRATION_NAME"
echo ""

dotnet ef migrations add "$MIGRATION_NAME" \
  --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj \
  --startup-project src/Wcs.Api/Wcs.Api.csproj

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Migration '$MIGRATION_NAME' đã được tạo thành công!"
    echo ""
    echo "📝 Để apply migration vào database, chạy:"
    echo "   dotnet ef database update --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj --startup-project src/Wcs.Api/Wcs.Api.csproj"
else
    echo ""
    echo "❌ Có lỗi xảy ra khi tạo migration"
    exit 1
fi

