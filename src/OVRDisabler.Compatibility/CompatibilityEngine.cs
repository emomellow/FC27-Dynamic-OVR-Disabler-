using Microsoft.Extensions.Logging;
using OVRDisabler.Core.Models;

namespace OVRDisabler.Compatibility;

public interface ICompatibilityEngine
{
    CompatibilityReport EvaluateCompatibility(GameInstallationInfo gameInfo);
}

public class CompatibilityEngine : ICompatibilityEngine
{
    private readonly ILogger<CompatibilityEngine>? _logger;

    // Bilinen ve hedeflenen oyun sürüm bilgileri
    public const string TargetGameTitle = "EA SPORTS FC 27 (Steam)";
    public const string TargetTitleUpdate = "Title Update v1.3.0";
    public const string TargetProductVersion = "27.0.9383086.0";
    public const string TargetFileVersion = "1.0.0.0";
    public const string TargetSteamBuildId = "26789738";

    public CompatibilityEngine(ILogger<CompatibilityEngine>? logger = null)
    {
        _logger = logger;
    }

    public CompatibilityReport EvaluateCompatibility(GameInstallationInfo gameInfo)
    {
        _logger?.LogInformation("Uyumluluk değerlendirmesi yapılıyor: IsFound={IsFound}, ProdVer={ProdVer}",
            gameInfo.IsFound, gameInfo.ProductVersion);

        if (!gameInfo.IsFound || string.IsNullOrEmpty(gameInfo.InstallPath))
        {
            return new CompatibilityReport(
                Status: GameCompatibilityStatus.GameFilesNotFound,
                StatusTurkish: GameCompatibilityStatus.GameFilesNotFound.ToDisplayString(),
                ExecutableVersion: "-",
                ProductVersion: "-",
                SteamBuildId: null,
                TargetGameTitle: TargetGameTitle,
                TargetUpdate: TargetTitleUpdate,
                CanDirectDeploy: false,
                ExplanationTurkish: "Oyun dosyaları bulunamadı. Lütfen Steam kurulumunu kontrol edin veya oyun klasörünü manuel olarak seçin."
            );
        }

        string currentProdVer = gameInfo.ProductVersion ?? string.Empty;
        string currentFileVer = gameInfo.FileVersion ?? string.Empty;
        string? currentBuildId = gameInfo.SteamBuildId;

        // Versiyon karşılaştırma mantığı
        if (string.Equals(currentProdVer, TargetProductVersion, StringComparison.OrdinalIgnoreCase))
        {
            // Tam eşleşen bilinen sürüm
            _logger?.LogInformation("Tam uyumlu sürüm tespit edildi: {Version}", currentProdVer);
            return new CompatibilityReport(
                Status: GameCompatibilityStatus.Compatible,
                StatusTurkish: GameCompatibilityStatus.Compatible.ToDisplayString(),
                ExecutableVersion: currentFileVer,
                ProductVersion: currentProdVer,
                SteamBuildId: currentBuildId,
                TargetGameTitle: TargetGameTitle,
                TargetUpdate: TargetTitleUpdate,
                CanDirectDeploy: false, // Bağımsız/güvenli doğrudan enjeksiyon mekanizması doğrulanana kadar false
                ExplanationTurkish: "Mevcut oyun sürümü (TU 1.3.0 - 27.0.9383086.0) CSV şeması ve mod parametreleri ile tam uyumludur. Doğrudan oyun içi dağıtım güvenliği EA Anticheat nedeniyle kısıtlıdır; CSV dışa aktarımı tam desteklenir."
            );
        }

        // Eğer versiyon numarası farklıysa
        if (int.TryParse(currentProdVer.Split('.')[0], out var major) && major >= 27)
        {
            _logger?.LogWarning("Farklı bir oyun güncellemesi tespit edildi: {Version}", currentProdVer);
            return new CompatibilityReport(
                Status: GameCompatibilityStatus.GameUpdateDetected,
                StatusTurkish: GameCompatibilityStatus.GameUpdateDetected.ToDisplayString(),
                ExecutableVersion: currentFileVer,
                ProductVersion: currentProdVer,
                SteamBuildId: currentBuildId,
                TargetGameTitle: TargetGameTitle,
                TargetUpdate: TargetTitleUpdate,
                CanDirectDeploy: false,
                ExplanationTurkish: $"Hedeflenen TU 1.3.0 sürümünden farklı bir oyun güncellemesi algılandı ({currentProdVer}). Oyun içi kilitlenme veya çökmeleri önlemek için doğrudan modlama engellenmiştir."
            );
        }

        return new CompatibilityReport(
            Status: GameCompatibilityStatus.Incompatible,
            StatusTurkish: GameCompatibilityStatus.Incompatible.ToDisplayString(),
            ExecutableVersion: currentFileVer,
            ProductVersion: currentProdVer,
            SteamBuildId: currentBuildId,
            TargetGameTitle: TargetGameTitle,
            TargetUpdate: TargetTitleUpdate,
            CanDirectDeploy: false,
            ExplanationTurkish: $"Oyun sürümü ({currentProdVer}) bu uygulama ile uyumsuz olarak değerlendirildi."
        );
    }
}
