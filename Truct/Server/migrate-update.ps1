# Script PowerShell de apply migration vao database
# Su dung: .\migrate-update.ps1

Write-Host "Dang apply migration vao database..." -ForegroundColor Cyan
Write-Host ""

dotnet ef database update `
  --project src/Wcs.Infrastructure/Wcs.Infrastructure.csproj `
  --startup-project src/Wcs.Api/Wcs.Api.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "Migration da duoc apply thanh cong!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "Co loi xay ra khi apply migration" -ForegroundColor Red
    exit 1
}

