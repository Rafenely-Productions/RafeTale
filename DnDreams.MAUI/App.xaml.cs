using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using DnDreams.Application.Services.Importer.Initializer;

namespace DnDreams.MAUI;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<App> _logger;

    public App(IServiceProvider serviceProvider, ILogger<App> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage())
        {
            Title = "DnDreams",
            Width = 410,
            Height = 850,
            MinimumWidth = 380,
            MinimumHeight = 650
        };
        return window;
    }

    protected override void OnStart()
    {
        base.OnStart();
        _ = InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            var initializer = _serviceProvider.GetRequiredService<IAppInitializer>();
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dndreams.db3");

            await initializer.InitializeAsync(async () =>
            {
                if (!File.Exists(dbPath) || new FileInfo(dbPath).Length == 0)
                {
                    initializer.UpdateStatus("Desempacando grimorio (Primera vez)...");

                    using Stream assetStream = await FileSystem.OpenAppPackageFileAsync("dndreams.db3");
                    using FileStream writeStream = File.OpenWrite(dbPath);

                    await assetStream.CopyToAsync(writeStream);
                    await writeStream.FlushAsync();
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize database");
        }
    }
}