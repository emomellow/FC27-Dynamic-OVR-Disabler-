using OVRDisabler.Backup;
using Xunit;

namespace OVRDisabler.GameData.Tests;

public class BackupServiceTests
{
    private readonly BackupService _backupService = new();

    [Fact]
    public void BackupAndRestore_MaintainsIntegrity()
    {
        string tempDir = Path.Combine(Path.GetTempPath(), "BackupTest_" + Guid.NewGuid().ToString("N"));
        string backupDir = Path.Combine(tempDir, "Backups");
        Directory.CreateDirectory(tempDir);

        try
        {
            string sourceFile = Path.Combine(tempDir, "test_file.csv");
            string originalContent = "Header1,Header2\nValue1,Value2";
            File.WriteAllText(sourceFile, originalContent);

            // 1. Yedek oluştur
            var backupResult = _backupService.CreateBackup(sourceFile, backupDir, "Birim testi yedeği");
            Assert.True(backupResult.Success);
            Assert.NotNull(backupResult.Manifest);
            Assert.True(File.Exists(backupResult.Manifest.BackupFilePath));

            // Manifest dosyasını bul
            var backups = _backupService.GetBackups(backupDir);
            Assert.Single(backups);

            string manifestPath = Directory.GetFiles(backupDir, "*.manifest.json").First();
            Assert.True(_backupService.VerifyBackupIntegrity(manifestPath));

            // 2. Kaynak dosyayı değiştir
            File.WriteAllText(sourceFile, "ModifiedContent,Corrupted");

            // 3. Geri yükle
            var restoreResult = _backupService.RestoreBackup(manifestPath, sourceFile);
            Assert.True(restoreResult.Success);

            // 4. Doğrula
            string restoredContent = File.ReadAllText(sourceFile);
            Assert.Equal(originalContent, restoredContent);
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }
}
