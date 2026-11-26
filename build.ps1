# Build Script
param(
    [string]$Configuration = "Release"
)

Write-Host "FC 27 Dynamic OVR Disabler derleniyor... [$Configuration]" -ForegroundColor Cyan
dotnet build FC27-Dynamic-OVR-Disabler.sln -c $Configuration

if ($LASTEXITCODE -eq 0) {
    Write-Host "Derleme basariyla tamamlandi!" -ForegroundColor Green
} else {
    Write-Host "Derleme sirasinda hata olustu!" -ForegroundColor Red
    exit $LASTEXITCODE
}
