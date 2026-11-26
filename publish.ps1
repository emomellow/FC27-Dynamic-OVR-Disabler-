# Publish Script
param(
    [string]$Version = "0.1.0-alpha"
)

$publishDir = "$PSScriptRoot\publish\FC27-Dynamic-OVR-Disabler-v$Version-win-x64"
Write-Host "Self-contained portable yayin hazirlaniyor: $publishDir" -ForegroundColor Cyan

if (Test-Path $publishDir) {
    Remove-Item -Recurse -Force $publishDir
}

dotnet publish src/OVRDisabler.App/OVRDisabler.App.csproj `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -o $publishDir

if ($LASTEXITCODE -eq 0) {
    Write-Host "Yayin basariyla olusturuldu!" -ForegroundColor Green
    
    # ZIP arsivi olustur
    $zipPath = "$PSScriptRoot\publish\FC27-Dynamic-OVR-Disabler-v$Version-win-x64.zip"
    if (Test-Path $zipPath) { Remove-Item -Force $zipPath }
    Compress-Archive -Path "$publishDir\*" -DestinationPath $zipPath -Force
    Write-Host "ZIP paketi olusturuldu: $zipPath" -ForegroundColor Green
} else {
    Write-Host "Publish sirasinda hata olustu!" -ForegroundColor Red
    exit $LASTEXITCODE
}
