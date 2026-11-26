using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OVRDisabler.App.ViewModels;
using OVRDisabler.Backup;
using OVRDisabler.Compatibility;
using OVRDisabler.Core.Logging;
using OVRDisabler.GameData;
using OVRDisabler.Steam;

namespace OVRDisabler.App;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();

        // Logging
        var inMemorySink = new InMemoryLogSink();
        services.AddSingleton(inMemorySink);
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddProvider(new AppInMemoryLoggerProvider(inMemorySink));
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Services
        services.AddSingleton<ISteamGameDetector, SteamGameDetector>();
        services.AddSingleton<ICompatibilityEngine, CompatibilityEngine>();
        services.AddSingleton<ICsvTransformationEngine, CsvTransformationEngine>();
        services.AddSingleton<IBackupService, BackupService>();

        // ViewModels & Views
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        var logger = _serviceProvider.GetRequiredService<ILogger<App>>();
        logger.LogInformation("FC 27 Dynamic OVR Disabler v0.1.0-alpha başlatıldı.");

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
