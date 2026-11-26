# Test Script
param(
    [string]$Configuration = "Release"
)

Write-Host "Tum testler calistiriliyor... [$Configuration]" -ForegroundColor Cyan
dotnet test FC27-Dynamic-OVR-Disabler.sln -c $Configuration --verbosity normal

if ($LASTEXITCODE -eq 0) {
    Write-Host "Tum testler basariyla gecti!" -ForegroundColor Green
} else {
    Write-Host "Testlerde basarisizlik tespit edildi!" -ForegroundColor Red
    exit $LASTEXITCODE
}
