using Microsoft.Extensions.Logging;
using Rafedream.Application.Services.Importer.Initializer;
using Rafedream.Infrastructure.Persistence;
using Rafedream.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Rafedream.MAUI;

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
            Title = "Rafedream",
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

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "rafedream.db3");
        _logger.LogInformation("DB path: {Path}", dbPath);

        try
        {
            var initializer = _serviceProvider.GetRequiredService<IAppInitializer>();

            await initializer.InitializeAsync(async () =>
            {
                // 1. CREAR SCHEMA SIEMPRE (independiente del Excel)
                var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
                using var scope = scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<RafedreamDbContext>();
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

                        using Stream excelStream = await FileSystem.OpenAppPackageFileAsync("Rafedream_SRD_v1.xlsx");
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

    private async Task<bool> CheckIfDatabaseHasDataAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RafedreamDbContext>();

            // Si hay al menos una clase, asumimos que la DB está poblada
            return await context.ClassDefinitions.AnyAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not check DB data. Assuming empty.");
            return false;
        }
    }

    private async Task ImportFromExcelAsync(IAppInitializer initializer)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var importService = scope.ServiceProvider.GetRequiredService<IExcelImportService>();

            initializer.UpdateStatus("Desempacando grimorio (Primera vez)...");

            using Stream excelStream = await FileSystem.OpenAppPackageFileAsync("Rafedream_v2.xlsx");

            initializer.UpdateStatus("Importando datos...");
            await importService.ImportDataFromExcelAsync(excelStream);

            _logger.LogInformation("Excel import completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel import failed. Is Rafedream_v2.xlsx in Resources/Raw/?");
            throw; // Relanzamos para que el initializer marque el error si quieres
        }
    }
}