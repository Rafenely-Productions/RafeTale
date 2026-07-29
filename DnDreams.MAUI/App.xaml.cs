using Microsoft.Extensions.Logging;
using DnDreams.Application.Services.Importer.Initializer;
using DnDreams.Infrastructure.Persistence;
using DnDreams.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        return new Window(new MainPage())
        {
            Title = "DnDreams",
            Width = 410,
            Height = 850,
            MinimumWidth = 380,
            MinimumHeight = 650
        };
    }

    protected override void OnStart()
    {
        base.OnStart();
        _ = InitializeDatabaseAsync();
    }

    private async Task InitializeDatabaseAsync()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dndreams.db3");
        _logger.LogInformation("DB path: {Path}", dbPath);

        try
        {
            var initializer = _serviceProvider.GetRequiredService<IAppInitializer>();

            await initializer.InitializeAsync(async () =>
            {
                // 1. Si no existe la DB, creamos el esquema (tablas vacías)
                if (!File.Exists(dbPath))
                {
                    _logger.LogInformation("DB not found. Creating schema...");
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<DnDreamsDbContext>();
                    await context.Database.EnsureCreatedAsync();
                }

                // 2. Verificamos si la DB ya tiene datos
                bool hasData = await CheckIfDatabaseHasDataAsync();

                if (!hasData)
                {
                    _logger.LogInformation("DB is empty. Importing from Excel...");
                    await ImportFromExcelAsync(initializer);
                }
                else
                {
                    _logger.LogInformation("DB already has data. Skipping import.");
                }

                initializer.UpdateStatus("¡Todo listo para la aventura!");
            });

            _logger.LogInformation("Initialization completed. IsDatabaseReady = {Ready}", initializer.IsDatabaseReady);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed");
        }
    }

    private async Task<bool> CheckIfDatabaseHasDataAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DnDreamsDbContext>();

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

            using Stream excelStream = await FileSystem.OpenAppPackageFileAsync("DnDreams_v2.xlsx");

            initializer.UpdateStatus("Importando datos de D&D...");
            await importService.ImportDataFromExcelAsync(excelStream);

            _logger.LogInformation("Excel import completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel import failed. Is DnDreams_v2.xlsx in Resources/Raw/?");
            throw; // Relanzamos para que el initializer marque el error si quieres
        }
    }
}