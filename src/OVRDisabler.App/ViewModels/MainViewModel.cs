using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OVRDisabler.Backup;
using OVRDisabler.Compatibility;
using OVRDisabler.Core.Localization;
using OVRDisabler.Core.Logging;
using OVRDisabler.Core.Models;
using OVRDisabler.GameData;
using OVRDisabler.Steam;

namespace OVRDisabler.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISteamGameDetector _steamDetector;
    private readonly ICompatibilityEngine _compatibilityEngine;
    private readonly ICsvTransformationEngine _csvEngine;
    private readonly IBackupService _backupService;
    private readonly InMemoryLogSink _logSink;
    private readonly ILogger<MainViewModel> _logger;

    public MainViewModel(
        ISteamGameDetector steamDetector,
        ICompatibilityEngine compatibilityEngine,
        ICsvTransformationEngine csvEngine,
        IBackupService backupService,
        InMemoryLogSink logSink,
        ILogger<MainViewModel> logger)
    {
        _steamDetector = steamDetector;
        _compatibilityEngine = compatibilityEngine;
        _csvEngine = csvEngine;
        _backupService = backupService;
        _logSink = logSink;
        _logger = logger;

        _logSink.OnLogAdded += entry =>
        {
            System.Windows.Application.Current?.Dispatcher.Invoke(() =>
            {
                Logs.Insert(0, $"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}] {entry.Message}");
                if (Logs.Count > 500) Logs.RemoveAt(Logs.Count - 1);
            });
        };

        // Başlangıç tespiti
        DetectGame();
        LoadBackups();
    }

    // Navigasyon
    [ObservableProperty] private string _currentView = "Overview";

    // Dil Desteği
    [ObservableProperty] private bool _isTurkish = true;
    public string this[string key] => LocalizationManager.Get(key);

    [RelayCommand]
    public void SetLanguage(string lang)
    {
        if (lang.Equals("en", StringComparison.OrdinalIgnoreCase))
        {
            LocalizationManager.CurrentLanguage = AppLanguage.English;
            IsTurkish = false;
        }
        else
        {
            LocalizationManager.CurrentLanguage = AppLanguage.Turkish;
            IsTurkish = true;
        }
        OnPropertyChanged(string.Empty); // Tüm binding'leri yenile
    }

    // Oyun ve Uyumluluk Durumu
    [ObservableProperty] private string _gameStatusText = "Oyun Aranıyor...";
    [ObservableProperty] private string _gameVersionText = "-";
    [ObservableProperty] private string _productVersionText = "-";
    [ObservableProperty] private string _compatibilityStatusText = "Henüz doğrulanmadı";
    [ObservableProperty] private string _compatibilityExplanation = string.Empty;
    [ObservableProperty] private string _steamPathText = "-";
    [ObservableProperty] private string _lastAnalysisDateText = "-";
    [ObservableProperty] private string _appVersionText = "0.1.0-alpha";
    [ObservableProperty] private bool _canDirectDeploy = false;

    // Dynamic OVR Seçenekleri
    [ObservableProperty] private bool _disableForm = true;
    [ObservableProperty] private bool _disableMorale = true;
    [ObservableProperty] private bool _disableMatchFitness = true;

    // CSV Analiz ve Dosya Bilgileri
    [ObservableProperty] private string _selectedCsvPath = string.Empty;
    [ObservableProperty] private string _analyzedFileName = "-";
    [ObservableProperty] private string _analyzedFileSize = "-";
    [ObservableProperty] private string _analyzedFileType = "-";
    [ObservableProperty] private string _analyzedSha256 = "-";
    [ObservableProperty] private string _analyzedSchemaStatus = "-";
    [ObservableProperty] private int _analyzedChangedCount = 0;
    [ObservableProperty] private string _analysisMessage = "Lütfen analiz edilecek bir CSV dosyası seçin.";

    public ObservableCollection<CsvValueChange> PreviewChangesList { get; } = new();
    public ObservableCollection<BackupManifest> BackupList { get; } = new();
    public ObservableCollection<string> Logs { get; } = new();

    private GameInstallationInfo? _currentGameInfo;

    [RelayCommand]
    public void SwitchView(string viewName)
    {
        CurrentView = viewName;
    }

    [RelayCommand]
    public void OpenCoffee()
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.buymeacoffee.com/emresfy",
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch { }
    }

    [RelayCommand]
    public void DetectGame()
    {
        _logger.LogInformation("Kullanıcı tarafından oyun tespiti tetiklendi.");
        _currentGameInfo = _steamDetector.DetectGame();
        UpdateGameStatus(_currentGameInfo);
    }

    [RelayCommand]
    public void SelectGameFolder(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        _logger.LogInformation("Kullanıcı manuel klasör seçti: {Path}", path);
        _currentGameInfo = _steamDetector.DetectGame(path);
        UpdateGameStatus(_currentGameInfo);
    }

    private void UpdateGameStatus(GameInstallationInfo info)
    {
        GameStatusText = info.Message;
        SteamPathText = info.InstallPath ?? "-";
        GameVersionText = info.FileVersion ?? "-";
        ProductVersionText = info.ProductVersion ?? "-";

        var report = _compatibilityEngine.EvaluateCompatibility(info);
        CompatibilityStatusText = report.StatusTurkish;
        CompatibilityExplanation = report.ExplanationTurkish;
        CanDirectDeploy = report.CanDirectDeploy;
    }

    [RelayCommand]
    public void AnalyzeFile(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            AnalysisMessage = "Geçersiz dosya seçimi.";
            return;
        }

        SelectedCsvPath = filePath;
        var options = new DisablerOptions(DisableForm, DisableMorale, DisableMatchFitness);
        var result = _csvEngine.Analyze(filePath, options);

        AnalyzedFileName = result.FileName;
        AnalyzedFileSize = $"{result.FileSizeBytes:N0} bayt";
        AnalyzedFileType = result.AssetType switch
        {
            CsvAssetType.FormMoraleBoosts => "DynamicOverall_FormMorale_Boosts.csv",
            CsvAssetType.MatchFitnessData => "DynamicOverall_MatchFitnessData.csv",
            _ => "Bilinmeyen / Desteklenmeyen Şema"
        };
        AnalyzedSha256 = result.Sha256Hash;
        AnalyzedSchemaStatus = result.IsSchemaValid ? "Doğrulandı" : "Geçersiz / Desteklenmiyor";
        AnalyzedChangedCount = result.TotalValuesChanged;
        AnalysisMessage = result.SchemaValidationMessage;
        LastAnalysisDateText = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

        PreviewChangesList.Clear();
        foreach (var change in result.PreviewChanges.Take(100))
        {
            PreviewChangesList.Add(change);
        }

        _logger.LogInformation("Dosya analiz edildi: {File}, Değişen Değer: {Count}", result.FileName, result.TotalValuesChanged);
    }

    [RelayCommand]
    public void ExportTransformedCsv(string targetPath)
    {
        if (string.IsNullOrWhiteSpace(SelectedCsvPath) || !File.Exists(SelectedCsvPath))
        {
            AnalysisMessage = "Önce dönüştürülecek bir CSV dosyası seçmelisiniz.";
            return;
        }

        try
        {
            var options = new DisablerOptions(DisableForm, DisableMorale, DisableMatchFitness);
            string content = File.ReadAllText(SelectedCsvPath);
            var assetType = _csvEngine.IdentifyAssetType(content);
            var result = _csvEngine.Transform(content, assetType, options);

            if (!result.Success || result.TransformedCsvContent == null)
            {
                AnalysisMessage = $"Dönüşüm başarısız: {result.Message}";
                return;
            }

            // Orijinal dosyanın yedeğini al
            var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            _backupService.CreateBackup(SelectedCsvPath, backupDir, "Dışa aktarma öncesi otomatik yedek");
            LoadBackups();

            File.WriteAllText(targetPath, result.TransformedCsvContent);
            AnalysisMessage = $"Dönüştürülmüş CSV başarıyla dışa aktarıldı: {Path.GetFileName(targetPath)}";
            _logger.LogInformation("CSV dışa aktarıldı: {Target}", targetPath);
        }
        catch (Exception ex)
        {
            AnalysisMessage = $"Dışa aktarma hatası: {ex.Message}";
            _logger.LogError(ex, "CSV dışa aktarma hatası");
        }
    }

    [RelayCommand]
    public void RestoreBackup(string? manifestPath)
    {
        if (string.IsNullOrWhiteSpace(manifestPath) || string.IsNullOrWhiteSpace(SelectedCsvPath)) return;
        var res = _backupService.RestoreBackup(manifestPath, SelectedCsvPath);
        AnalysisMessage = res.Message;
        LoadBackups();
    }

    public void LoadBackups()
    {
        BackupList.Clear();
        var backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        var list = _backupService.GetBackups(backupDir);
        foreach (var item in list)
        {
            BackupList.Add(item);
        }
    }
}
