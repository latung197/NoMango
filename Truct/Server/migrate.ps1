# Script PowerShell để chạy migration từ root directory
# Sử dụng: .\migrate.ps1 [migration-name]

param(
    [string]$MigrationName = "PendingModelChanges"
)

Write-Host "Tao migration: $MigrationName" -ForegroundColor Cyan
Write-Host ""

dotnet ef migrations add $MigrationName `
  --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj `
  --startup-project src/Wcs.Api/Wcs.Api.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Migration '$MigrationName' da duoc tao thanh cong!" -ForegroundColor Green
    Write-Host ""
    Write-Host "De apply migration vao database, chay:" -ForegroundColor Yellow
    Write-Host "   dotnet ef database update --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj --startup-project src/Wcs.Api/Wcs.Api.csproj"
} else {
    Write-Host ""
    Write-Host "Co loi xay ra khi tao migration" -ForegroundColor Red
    exit 1
}
