using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Application.Services;
using DnDreams.Application.Services.DtosServices;
using DnDreams.Domain.DTOs;
using DnDreams.Domain.Entities;
using DnDreams.Domain.Interfaces;
using DnDreams.Infrastructure;
using DnDreams.Infrastructure.Extractors;
using DnDreams.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
namespace DnDreams.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        builder.Services.AddMauiBlazorWebView();


        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dndreams.db3");

        builder.Services.AddInfrastructure(dbPath);
        builder.Services.AddScoped<ILevelingService, LevelingService>();

        builder.Services.AddScoped<ICharacterQueryService, CharacterQueryService>();
        builder.Services.AddScoped<IFeatureQueryService, FeatureQueryService>();

        builder.Services.AddScoped<CharacterCreationService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IDataExtractor, ExcelDataExtractor>();
        builder.Services.AddScoped<IExcelImportService, ImportManager>();
        builder.Services.AddScoped<ILocalizationService, LocalizationService>();
        builder.Services.AddScoped<IService<RaceDto, Race>, RaceService>();
        builder.Services.AddScoped<IService<ClassDefinitionDto, ClassDefinition>, ClassService>();
        builder.Services.AddScoped<IService<SpellDto, Spell>, SpellService>();
        builder.Services.AddScoped<IService<LanguageDto, Language>, LanguageService>();

        var culture = new CultureInfo("es-MX");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        builder.Services.AddLocalization();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = (Exception)args.ExceptionObject;
            System.Diagnostics.Debug.WriteLine($"¡CRASH EVITADO!: {ex.Message}");
        };
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DnDreams.Infrastructure.Persistence.DnDreamsDbContext>();
            //if (File.Exists(dbPath)) File.Delete(dbPath);
            Task.Run(async () => await InitializeDatabaseFromExcelAsync(app.Services));
        }

        return app;
    }

    // Lógica conceptual que se integra en tu capa de persistencia/arranque
    public static async Task InitializeDatabaseFromExcelAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DnDreamsDbContext>();
        try
        {
            await context.Database.MigrateAsync();

            if (await context.ClassDefinitions.AnyAsync() || await context.Spells.AnyAsync())
            {
                return;
            }

            var importManager = scope.ServiceProvider.GetRequiredService<IExcelImportService>();


            using Stream excelStream = await FileSystem.OpenAppPackageFileAsync("DnDreams_v2.xlsx");

            await importManager.ImportDataFromExcelAsync(excelStream);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error crítico en carga inicial de Excel: {ex.Message}");
        }
    }
}
