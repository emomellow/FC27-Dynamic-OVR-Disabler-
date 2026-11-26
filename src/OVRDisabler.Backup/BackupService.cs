using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OVRDisabler.Core.Models;
using OVRDisabler.Core.Utilities;

namespace OVRDisabler.Backup;

public interface IBackupService
{
    BackupResult CreateBackup(string sourceFilePath, string backupDirectory, string description);
    RestoreResult RestoreBackup(string manifestPath, string targetFilePath);
    IReadOnlyList<BackupManifest> GetBackups(string backupDirectory);
    bool VerifyBackupIntegrity(string manifestPath);
}

public class BackupService : IBackupService
{
    private readonly ILogger<BackupService>? _logger;

    public BackupService(ILogger<BackupService>? logger = null)
    {
        _logger = logger;
    }

    public BackupResult CreateBackup(string sourceFilePath, string backupDirectory, string description)
    {
        try
        {
            if (!File.Exists(sourceFilePath))
            {
                return new BackupResult(false, null, $"Yedeklenecek kaynak dosya bulunamadı: {sourceFilePath}");
            }

            Directory.CreateDirectory(backupDirectory);

            string fileHash = HashUtility.ComputeSha256(sourceFilePath);
            var fileInfo = new FileInfo(sourceFilePath);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = Path.GetFileName(sourceFilePath);
            string backupFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}{Path.GetExtension(fileName)}.bak";
            string backupFilePath = Path.Combine(backupDirectory, backupFileName);

            // Asla sessizce ezme: dosya zaten varsa benzersiz GUID ekle
            if (File.Exists(backupFilePath))
            {
                backupFilePath = Path.Combine(backupDirectory, $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}_{Guid.NewGuid():N}.bak");
            }

            // Güvenli atomik kopyalama
            string tempBackupPath = backupFilePath + ".tmp";
            File.Copy(sourceFilePath, tempBackupPath, true);

            // Hash doğrulaması yap
            string tempHash = HashUtility.ComputeSha256(tempBackupPath);
            if (!string.Equals(fileHash, tempHash, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(tempBackupPath);
                return new BackupResult(false, null, "Kopyalanan yedek dosyasının SHA-256 bütünlüğü eşleşmedi! İşlem iptal edildi.");
            }

            File.Move(tempBackupPath, backupFilePath);

            string backupId = Guid.NewGuid().ToString("N");
            var manifest = new BackupManifest(
                BackupId: backupId,
                Timestamp: DateTime.Now,
                OriginalFilePath: Path.GetFullPath(sourceFilePath),
                BackupFilePath: Path.GetFullPath(backupFilePath),
                Sha256Original: fileHash,
                FileSize: fileInfo.Length,
                Description: description
            );

            string manifestPath = Path.Combine(backupDirectory, $"{Path.GetFileNameWithoutExtension(backupFileName)}.manifest.json");
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, jsonOptions));

            _logger?.LogInformation("Başarılı yedek oluşturuldu: {BackupFilePath}, Manifest: {ManifestPath}", backupFilePath, manifestPath);
            return new BackupResult(true, manifest, "Yedek başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Yedek oluşturma sırasında hata oluştu: {File}", sourceFilePath);
            return new BackupResult(false, null, $"Yedekleme hatası: {ex.Message}");
        }
    }

    public RestoreResult RestoreBackup(string manifestPath, string targetFilePath)
    {
        try
        {
            if (!File.Exists(manifestPath))
            {
                return new RestoreResult(false, targetFilePath, "Yedek manifest dosyası bulunamadı.");
            }

            var manifest = JsonSerializer.Deserialize<BackupManifest>(File.ReadAllText(manifestPath));
            if (manifest == null || !File.Exists(manifest.BackupFilePath))
            {
                return new RestoreResult(false, targetFilePath, "Yedek dosyası veya manifest içeriği geçersiz.");
            }

            // Bütünlük kontrolü
            string currentBackupHash = HashUtility.ComputeSha256(manifest.BackupFilePath);
            if (!string.Equals(currentBackupHash, manifest.Sha256Original, StringComparison.OrdinalIgnoreCase))
            {
                return new RestoreResult(false, targetFilePath, "Yedek dosyasının bütünlüğü bozulmuş! Geri yükleme iptal edildi.");
            }

            // Hedef dizini hazırla
            var targetDir = Path.GetDirectoryName(targetFilePath);
            if (!string.IsNullOrEmpty(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // Güvenli geri yükleme: önce geçici dosyaya kopyala
            string tempRestore = targetFilePath + ".restore.tmp";
            File.Copy(manifest.BackupFilePath, tempRestore, true);

            string restoredHash = HashUtility.ComputeSha256(tempRestore);
            if (!string.Equals(restoredHash, manifest.Sha256Original, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(tempRestore);
                return new RestoreResult(false, targetFilePath, "Geri yükleme kopyalama doğrulaması başarısız.");
            }

            File.Move(tempRestore, targetFilePath, true);

            _logger?.LogInformation("Dosya başarıyla geri yüklendi: {TargetFilePath} (Kaynak: {BackupPath})", targetFilePath, manifest.BackupFilePath);
            return new RestoreResult(true, targetFilePath, "Dosya başarıyla orijinal yedeğinden geri yüklendi.");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Geri yükleme hatası: {ManifestPath}", manifestPath);
            return new RestoreResult(false, targetFilePath, $"Geri yükleme hatası: {ex.Message}");
        }
    }

    public IReadOnlyList<BackupManifest> GetBackups(string backupDirectory)
    {
        var list = new List<BackupManifest>();
        if (!Directory.Exists(backupDirectory)) return list;

        try
        {
            var manifestFiles = Directory.GetFiles(backupDirectory, "*.manifest.json");
            foreach (var file in manifestFiles)
            {
                try
                {
                    var manifest = JsonSerializer.Deserialize<BackupManifest>(File.ReadAllText(file));
                    if (manifest != null)
                    {
                        list.Add(manifest);
                    }
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Yedekler listelenirken hata oluştu.");
        }

        return list.OrderByDescending(b => b.Timestamp).ToList();
    }

    public bool VerifyBackupIntegrity(string manifestPath)
    {
        try
        {
            if (!File.Exists(manifestPath)) return false;
            var manifest = JsonSerializer.Deserialize<BackupManifest>(File.ReadAllText(manifestPath));
            if (manifest == null || !File.Exists(manifest.BackupFilePath)) return false;

            string currentHash = HashUtility.ComputeSha256(manifest.BackupFilePath);
            return string.Equals(currentHash, manifest.Sha256Original, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
