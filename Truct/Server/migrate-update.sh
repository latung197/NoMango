#!/bin/bash

# Script để apply migration vào database
# Sử dụng: ./migrate-update.sh

echo "🔄 Đang apply migration vào database..."
echo ""

dotnet ef database update \
  --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj \
  --startup-project src/Wcs.Api/Wcs.Api.csproj

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Migration đã được apply thành công!"
else
    echo ""
    echo "❌ Có lỗi xảy ra khi apply migration"
    exit 1
fi

