using Microsoft.Extensions.Logging;
using RafeTale.Application.Services.Importer.Initializer;
using RafeTale.Infrastructure.Persistence;
using RafeTale.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace RafeTale.MAUI;

public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<App> _logger;
    private static bool _hasInitialized = false;

    public App(IServiceProvider serviceProvider, ILogger<App> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage())
        {
            Title = "RafeTale",
            Width = 410,
            Height = 850,
            MinimumWidth = 380,
            MinimumHeight = 650
        };
    }

    protected override void OnStart()
    {
        base.OnStart();
        if (!_hasInitialized)
        {
            _hasInitialized = true;
            _ = Task.Run(async () => await InitializeDatabaseAsync());
        }
    }

    private async Task InitializeDatabaseAsync()
    {
        await Task.Yield();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "rafetale.db3");
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("DB path: {Path}", dbPath);
        }
        try
        {
            var initializer = _serviceProvider.GetRequiredService<IAppInitializer>();

            await initializer.InitializeAsync(async () =>
            {
                // 1. CREAR SCHEMA SIEMPRE (independiente del Excel)
                var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<RafeTaleDbContext>();
                await context.Database.EnsureCreatedAsync();

                // 2. Verificar si ya tiene datos
                bool hasData = await context.ClassDefinitions.AnyAsync();

                if (!hasData)
                {
                    // 3. Intentar importar Excel (pero no romper si falla)
                    try
                    {
                        var importService = scope.ServiceProvider.GetRequiredService<IExcelImportService>();
                        initializer.UpdateStatus("Importando datos...");

                        using Stream excelStream = await FileSystem.OpenAppPackageFileAsync("RafeTale_SRD_v1.xlsx");
                        await importService.ImportDataFromExcelAsync(excelStream);

                        _logger.LogInformation("Excel import completed.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Excel import failed. DB created but empty.");
                        initializer.UpdateStatus("Listo (sin datos). Importa un pack para comenzar.");
                    }
                }
                else
                {
                    _logger.LogInformation("DB already has data.");
                }

                initializer.UpdateStatus("¡Todo listo!");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical initialization error");
        }
    }
}