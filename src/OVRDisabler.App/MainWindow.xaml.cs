using System.Windows;
using Microsoft.Win32;
using OVRDisabler.App.ViewModels;

namespace OVRDisabler.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // İkon dosyasını hazırla
        var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "icon.ico");
        IconGenerator.EnsureIcon(iconPath);
    }

    private void SelectFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "EA SPORTS FC 27 Kurulum Klasörünü Seçin"
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.SelectGameFolder(dialog.FolderName);
        }
    }

    private void SelectFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Dynamic OVR CSV Dosyası Seçin",
            Filter = "CSV Dosyaları (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.AnalyzeFile(dialog.FileName);
            _viewModel.SwitchView("Analysis");
        }
    }

    private void ExportCsv_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_viewModel.SelectedCsvPath))
        {
            MessageBox.Show("Lütfen önce bir CSV dosyası seçin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var dialog = new SaveFileDialog
        {
            Title = "Dönüştürülmüş CSV Dosyasını Kaydet",
            Filter = "CSV Dosyaları (*.csv)|*.csv|Tüm Dosyalar (*.*)|*.*",
            FileName = System.IO.Path.GetFileName(_viewModel.SelectedCsvPath)
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.ExportTransformedCsv(dialog.FileName);
            MessageBox.Show("Dönüştürülmüş CSV dosyası başarıyla dışa aktarıldı!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void RestoreOVR_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.SwitchView("Backups");
    }
}
