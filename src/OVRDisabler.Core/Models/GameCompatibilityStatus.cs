namespace OVRDisabler.Core.Models;

public enum GameCompatibilityStatus
{
    Compatible,             // Uyumlu
    Incompatible,           // Uyumsuz
    NotYetVerified,         // Henüz doğrulanmadı
    GameUpdateDetected,     // Oyun güncellemesi algılandı
    GameFilesNotFound       // Oyun dosyaları bulunamadı
}

public static class GameCompatibilityStatusExtensions
{
    public static string ToDisplayString(this GameCompatibilityStatus status) => status switch
    {
        GameCompatibilityStatus.Compatible => "Uyumlu",
        GameCompatibilityStatus.Incompatible => "Uyumsuz",
        GameCompatibilityStatus.NotYetVerified => "Henüz doğrulanmadı",
        GameCompatibilityStatus.GameUpdateDetected => "Oyun güncellemesi algılandı",
        GameCompatibilityStatus.GameFilesNotFound => "Oyun dosyaları bulunamadı",
        _ => "Bilinmiyor"
    };
}
