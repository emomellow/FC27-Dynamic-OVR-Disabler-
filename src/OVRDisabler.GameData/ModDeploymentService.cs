using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using OVRDisabler.Core.Models;

namespace OVRDisabler.GameData;

public interface IModDeploymentService
{
    bool CanDeploy(string gamePath, out string reason);
    bool DeployMod(string gamePath, string sourceModPath, int targetChangelist, out string message);
}

public class ModDeploymentService : IModDeploymentService
{
    private readonly ILogger<ModDeploymentService>? _logger;
    public const int TargetChangelistTu130 = 9383086;
    public const int PreviousChangelist = 9326777;

    public ModDeploymentService(ILogger<ModDeploymentService>? logger = null)
    {
        _logger = logger;
    }

    public bool CanDeploy(string gamePath, out string reason)
    {
        if (!Directory.Exists(gamePath))
        {
            reason = "Oyun klasörü bulunamadı.";
            return false;
        }

        var exePath = Path.Combine(gamePath, "FC27.exe");
        if (!File.Exists(exePath))
        {
            reason = "FC27.exe bulunamadı.";
            return false;
        }

        var fifaModData = Path.Combine(gamePath, "FIFAModData");
        if (!Directory.Exists(fifaModData))
        {
            reason = "FIFAModData klasörü bulunamadı. Lütfen önce modlama ortamını hazır bulundurun.";
            return false;
        }

        reason = "Oyun dizini ve modlama ortamı TU 1.3.0 yaması için hazır.";
        return true;
    }

    public bool DeployMod(string gamePath, string sourceModPath, int targetChangelist, out string message)
    {
        try
        {
            _logger?.LogInformation("Mod güncelleme ve dağıtım işlemi başlatıldı. Hedef changelist: {Changelist}", targetChangelist);

            var fifaModData = Path.Combine(gamePath, "FIFAModData");
            if (!Directory.Exists(fifaModData))
            {
                message = "FIFAModData dizini bulunamadı.";
                return false;
            }

            // 1. Masaüstündeki .fifamod dosyasının changelist'ini güncelle
            if (File.Exists(sourceModPath))
            {
                var modBytes = File.ReadAllBytes(sourceModPath);
                var oldBytes = BitConverter.GetBytes(PreviousChangelist);
                var newBytes = BitConverter.GetBytes(targetChangelist);

                for (int i = 0; i < modBytes.Length - 4; i++)
                {
                    if (modBytes[i] == oldBytes[0] && modBytes[i + 1] == oldBytes[1] &&
                        modBytes[i + 2] == oldBytes[2] && modBytes[i + 3] == oldBytes[3])
                    {
                        Array.Copy(newBytes, 0, modBytes, i, 4);
                        _logger?.LogInformation("sourceModPath içinde changelist güncellendi @ {Offset}", i);
                    }
                }
                File.WriteAllBytes(sourceModPath, modBytes);
            }

            // 2. FIFAModData/Mods.json dosyasını güncelle
            var modsJsonPath = Path.Combine(fifaModData, "Mods.json");
            if (File.Exists(modsJsonPath))
            {
                var jsonContent = File.ReadAllText(modsJsonPath);
                var jsonObj = JsonNode.Parse(jsonContent);
                if (jsonObj != null)
                {
                    jsonObj["ExeChangelist"] = targetChangelist;
                    File.WriteAllText(modsJsonPath, jsonObj.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));
                    _logger?.LogInformation("Mods.json ExeChangelist güncellendi -> {Changelist}", targetChangelist);
                }
            }

            // 3. FIFAModData içerisindeki ikili dosyalardaki changelist değerlerini güncelle
            var files = Directory.GetFiles(fifaModData, "*.*", SearchOption.AllDirectories);
            var prevBytes = BitConverter.GetBytes(PreviousChangelist);
            var targetBytes = BitConverter.GetBytes(targetChangelist);

            int patchedCount = 0;
            foreach (var file in files)
            {
                if (file.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) continue;

                var data = File.ReadAllBytes(file);
                bool modified = false;

                for (int i = 0; i < data.Length - 4; i++)
                {
                    if (data[i] == prevBytes[0] && data[i + 1] == prevBytes[1] &&
                        data[i + 2] == prevBytes[2] && data[i + 3] == prevBytes[3])
                    {
                        Array.Copy(targetBytes, 0, data, i, 4);
                        modified = true;
                        patchedCount++;
                    }
                }

                if (modified)
                {
                    File.WriteAllBytes(file, data);
                    _logger?.LogInformation("İkili dosya güncellendi: {File}", Path.GetFileName(file));
                }
            }

            message = $"Başarılı! Mod paketi ve FIFAModData, EA SPORTS FC 27 TU 1.3.0 (Changelist: {targetChangelist}) için uyarlandı ve etkinleştirildi.";
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Mod dağıtımı sırasında hata");
            message = $"Dağıtım hatası: {ex.Message}";
            return false;
        }
    }
}
