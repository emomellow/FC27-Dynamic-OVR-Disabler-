using OVRDisabler.Backup;
using OVRDisabler.Compatibility;
using OVRDisabler.Core.Models;
using OVRDisabler.GameData;
using OVRDisabler.Steam;
using Xunit;

namespace OVRDisabler.Integration.Tests;

public class EndToEndPipelineTests
{
    [Fact]
    public void FullWorkflow_Detect_Analyze_Transform_Backup_Restore()
    {
        // 1. Servisleri oluştur
        var steamDetector = new SteamGameDetector();
        var compatibilityEngine = new CompatibilityEngine();
        var csvEngine = new CsvTransformationEngine();
        var backupService = new BackupService();

        // 2. Oyun tespitini test et (Varsayılan veya kurulu Steam tespiti)
        var gameInfo = steamDetector.DetectGame();
        var report = compatibilityEngine.EvaluateCompatibility(gameInfo);

        Assert.NotNull(report);
        Assert.NotNull(report.StatusTurkish);

        // 3. Sentetik CSV ile uçtan uca modifikasyon ve yedek testi
        string tempWorkDir = Path.Combine(Path.GetTempPath(), "PipelineTest_" + Guid.NewGuid().ToString("N"));
        string backupDir = Path.Combine(tempWorkDir, "Backups");
        Directory.CreateDirectory(tempWorkDir);

        try
        {
            string dummyCsv = Path.Combine(tempWorkDir, "DynamicOverall_FormMorale_Boosts.csv");
            string originalContent =
@"# FC 27 Boosts
Category,Threshold,SkillWeight,PhysicalWeight,MentalWeight
Skill Weight,50,,,
Physical Weight,30,,,
Mental Weight,20,,,
Form,Threshold,SkillMod,PhysicalMod,MentalMod
Very Good,85,5,4,3
Very Bad,15,-5,-4,-3
Morale,Threshold,SkillMod,PhysicalMod,MentalMod
Very Happy,85,4,3,2
Very Sad,15,-4,-3,-2";

            File.WriteAllText(dummyCsv, originalContent);

            // Analiz
            var options = new DisablerOptions(DisableForm: true, DisableMorale: true, DisableMatchFitness: true);
            var analysis = csvEngine.Analyze(dummyCsv, options);

            Assert.True(analysis.IsSchemaValid);
            Assert.Equal(CsvAssetType.FormMoraleBoosts, analysis.AssetType);
            Assert.True(analysis.TotalValuesChanged > 0);

            // Yedekle
            var backupRes = backupService.CreateBackup(dummyCsv, backupDir, "İşlem öncesi otomatik yedek");
            Assert.True(backupRes.Success);

            // Dönüştür ve yaz
            var transformRes = csvEngine.Transform(originalContent, analysis.AssetType, options);
            Assert.True(transformRes.Success);
            File.WriteAllText(dummyCsv, transformRes.TransformedCsvContent!);

            // Dönüşümü doğrula
            string modified = File.ReadAllText(dummyCsv);
            Assert.Contains("Very Good,85,0,0,0", modified);
            Assert.Contains("Skill Weight,50", modified);

            // Geri yükle
            string manifestPath = Directory.GetFiles(backupDir, "*.manifest.json").First();
            var restoreRes = backupService.RestoreBackup(manifestPath, dummyCsv);
            Assert.True(restoreRes.Success);

            // Orijinale döndüğünü doğrula
            string restored = File.ReadAllText(dummyCsv);
            Assert.Equal(originalContent, restored);
        }
        finally
        {
            if (Directory.Exists(tempWorkDir)) Directory.Delete(tempWorkDir, true);
        }
    }
}
