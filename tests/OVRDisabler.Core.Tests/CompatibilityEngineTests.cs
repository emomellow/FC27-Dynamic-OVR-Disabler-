using OVRDisabler.Compatibility;
using OVRDisabler.Core.Models;
using Xunit;

namespace OVRDisabler.Core.Tests;

public class CompatibilityEngineTests
{
    private readonly CompatibilityEngine _engine = new();

    [Fact]
    public void WhenGameNotFound_ReturnsGameFilesNotFound()
    {
        var info = new GameInstallationInfo(false, null, null, null, null, null, null, "Test", "Oyun bulunamadı.");
        var report = _engine.EvaluateCompatibility(info);

        Assert.Equal(GameCompatibilityStatus.GameFilesNotFound, report.Status);
        Assert.Equal("Oyun dosyaları bulunamadı", report.StatusTurkish);
        Assert.False(report.CanDirectDeploy);
    }

    [Fact]
    public void WhenGameIsTargetVersion_ReturnsCompatible()
    {
        var info = new GameInstallationInfo(
            IsFound: true,
            InstallPath: @"C:\Games\FC 27",
            ExecutablePath: @"C:\Games\FC 27\FC27.exe",
            FileVersion: "1.0.0.0",
            ProductVersion: "27.0.9383086.0",
            SteamAppId: "4080220",
            SteamBuildId: "26789738",
            DetectionSource: "Test",
            Message: "Oyun bulundu."
        );

        var report = _engine.EvaluateCompatibility(info);

        Assert.Equal(GameCompatibilityStatus.Compatible, report.Status);
        Assert.Equal("Uyumlu", report.StatusTurkish);
        Assert.False(report.CanDirectDeploy); // Anticheat nedeniyle güvenli doğrudan deploy kısıtlı
    }

    [Fact]
    public void WhenGameVersionDiffers_ReturnsGameUpdateDetected()
    {
        var info = new GameInstallationInfo(
            IsFound: true,
            InstallPath: @"C:\Games\FC 27",
            ExecutablePath: @"C:\Games\FC 27\FC27.exe",
            FileVersion: "1.0.0.0",
            ProductVersion: "27.1.1000000.0", // Yeni güncelleme
            SteamAppId: "4080220",
            SteamBuildId: "99999999",
            DetectionSource: "Test",
            Message: "Oyun bulundu."
        );

        var report = _engine.EvaluateCompatibility(info);

        Assert.Equal(GameCompatibilityStatus.GameUpdateDetected, report.Status);
        Assert.Equal("Oyun güncellemesi algılandı", report.StatusTurkish);
        Assert.False(report.CanDirectDeploy);
    }

    [Fact]
    public void WhenGameVersionIsUnrelated_ReturnsIncompatible()
    {
        var info = new GameInstallationInfo(
            IsFound: true,
            InstallPath: @"C:\Games\FC 27",
            ExecutablePath: @"C:\Games\FC 27\FC27.exe",
            FileVersion: "1.0.0.0",
            ProductVersion: "12.0.0.0",
            SteamAppId: "4080220",
            SteamBuildId: null,
            DetectionSource: "Test",
            Message: "Oyun bulundu."
        );

        var report = _engine.EvaluateCompatibility(info);

        Assert.Equal(GameCompatibilityStatus.Incompatible, report.Status);
        Assert.Equal("Uyumsuz", report.StatusTurkish);
        Assert.False(report.CanDirectDeploy);
    }
}
