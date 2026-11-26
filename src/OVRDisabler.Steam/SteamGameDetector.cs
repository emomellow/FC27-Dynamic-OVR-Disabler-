using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using OVRDisabler.Core.Models;

namespace OVRDisabler.Steam;

public interface ISteamGameDetector
{
    GameInstallationInfo DetectGame(string? manualPath = null);
    IReadOnlyList<string> FindAllSteamLibraries();
}

public class SteamGameDetector : ISteamGameDetector
{
    private readonly ILogger<SteamGameDetector>? _logger;
    private const string TargetFolderName = "EA SPORTS FC 27";
    private const string TargetExeName = "FC27.exe";
    public const string TargetSteamAppId = "4080220";

    public SteamGameDetector(ILogger<SteamGameDetector>? logger = null)
    {
        _logger = logger;
    }

    public GameInstallationInfo DetectGame(string? manualPath = null)
    {
        _logger?.LogInformation("Oyun algılama işlemi başlatıldı. Manuel yol: {ManualPath}", manualPath ?? "(Yok)");

        if (!string.IsNullOrWhiteSpace(manualPath))
        {
            return ValidateDirectory(manualPath, "Manuel Seçim");
        }

        // 1. Steam Kütüphane Yollarını Tara
        var libraries = FindAllSteamLibraries();
        foreach (var lib in libraries)
        {
            var candidate = Path.Combine(lib, "steamapps", "common", TargetFolderName);
            if (Directory.Exists(candidate))
            {
                var info = ValidateDirectory(candidate, "Steam Kütüphanesi Otomatik Tespiti");
                if (info.IsFound) return info;
            }
        }

        // 2. Windows Registry Kontrolü (Varsayılan Steam Yolu)
        var defaultSteamCommon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", TargetFolderName);
        if (Directory.Exists(defaultSteamCommon))
        {
            var info = ValidateDirectory(defaultSteamCommon, "Varsayılan Steam Dizini");
            if (info.IsFound) return info;
        }

        _logger?.LogWarning("EA SPORTS FC 27 kurulumu tespit edilemedi.");
        return new GameInstallationInfo(
            IsFound: false,
            InstallPath: null,
            ExecutablePath: null,
            FileVersion: null,
            ProductVersion: null,
            SteamAppId: null,
            SteamBuildId: null,
            DetectionSource: "Otomatik Tarama",
            Message: "Oyun bulunamadı."
        );
    }

    public IReadOnlyList<string> FindAllSteamLibraries()
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        try
        {
            string? steamPath = null;
            if (OperatingSystem.IsWindows())
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
                if (key != null)
                {
                    steamPath = key.GetValue("SteamPath") as string;
                }
            }

            if (string.IsNullOrEmpty(steamPath) || !Directory.Exists(steamPath))
            {
                var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");
                if (Directory.Exists(defaultPath))
                {
                    steamPath = defaultPath;
                }
            }

            if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
            {
                result.Add(Path.GetFullPath(steamPath));
                var vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                if (!File.Exists(vdfPath))
                {
                    vdfPath = Path.Combine(steamPath, "config", "libraryfolders.vdf");
                }

                if (File.Exists(vdfPath))
                {
                    ParseLibraryFoldersVdf(vdfPath, result);
                }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Steam kütüphane tespiti sırasında hata oluştu.");
        }

        return result.ToList();
    }

    public static void ParseLibraryFoldersVdf(string vdfPath, HashSet<string> libraries)
    {
        try
        {
            var lines = File.ReadAllLines(vdfPath);
            var regex = new Regex(@"""path""\s+""([^""]+)""", RegexOptions.IgnoreCase);

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (match.Success)
                {
                    var rawPath = match.Groups[1].Value.Replace(@"\\", @"\");
                    if (Directory.Exists(rawPath))
                    {
                        libraries.Add(Path.GetFullPath(rawPath));
                    }
                }
            }
        }
        catch
        {
            // Yutma veya loglama
        }
    }

    public GameInstallationInfo ValidateDirectory(string directoryPath, string source)
    {
        try
        {
            if (!Directory.Exists(directoryPath))
            {
                return new GameInstallationInfo(false, directoryPath, null, null, null, null, null, source, "Geçersiz oyun klasörü.");
            }

            var exePath = Path.Combine(directoryPath, TargetExeName);
            if (!File.Exists(exePath))
            {
                return new GameInstallationInfo(false, directoryPath, null, null, null, null, null, source, "FC27.exe bulunamadı (Geçersiz oyun klasörü).");
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(exePath);
            string fileVer = versionInfo.FileVersion ?? "1.0.0.0";
            string prodVer = versionInfo.ProductVersion ?? "Bilinmiyor";

            // Steam App ID ve Build ID oku
            string? appId = null;
            string? buildId = null;

            var appIdFile = Path.Combine(directoryPath, "steam_appid.txt");
            if (File.Exists(appIdFile))
            {
                appId = File.ReadAllText(appIdFile).Trim();
            }

            // Üst dizindeki appmanifest'i kontrol et
            var parent = Directory.GetParent(directoryPath);
            if (parent != null && parent.Name.Equals("common", StringComparison.OrdinalIgnoreCase))
            {
                var steamAppsDir = parent.Parent;
                if (steamAppsDir != null)
                {
                    var targetManifest = Path.Combine(steamAppsDir.FullName, $"appmanifest_{appId ?? TargetSteamAppId}.acf");
                    if (File.Exists(targetManifest))
                    {
                        buildId = ExtractBuildIdFromManifest(targetManifest);
                    }
                }
            }

            _logger?.LogInformation("Oyun doğrulandı: {Path}, Sürüm: {ProdVer}, BuildId: {BuildId}", directoryPath, prodVer, buildId);

            return new GameInstallationInfo(
                IsFound: true,
                InstallPath: directoryPath,
                ExecutablePath: exePath,
                FileVersion: fileVer,
                ProductVersion: prodVer,
                SteamAppId: appId ?? TargetSteamAppId,
                SteamBuildId: buildId,
                DetectionSource: source,
                Message: "Oyun bulundu."
            );
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Dizin doğrulama hatası: {Path}", directoryPath);
            return new GameInstallationInfo(false, directoryPath, null, null, null, null, null, source, $"Hata: {ex.Message}");
        }
    }

    public static string? ExtractBuildIdFromManifest(string manifestPath)
    {
        try
        {
            var content = File.ReadAllText(manifestPath);
            var match = Regex.Match(content, @"""buildid""\s+""(\d+)""", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }
        catch { }
        return null;
    }
}
