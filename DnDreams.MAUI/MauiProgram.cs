using DnDreams.Application.DTOs;
using DnDreams.Application.Interfaces;
using DnDreams.Application.Interfaces.DtosInterfaces;
using DnDreams.Application.Services;
using DnDreams.Application.Services.DtosServices;
using DnDreams.Application.Services.Importer;
using DnDreams.Application.Services.Importer.Initializer;
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
        builder.Services.AddSingleton<IAppInitializer, AppInitializer>();

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
        builder.Services.AddScoped<IService<SubclassDto, Subclass>, SubclassService>();
        builder.Services.AddScoped<IService<LanguageDto, Language>, LanguageService>();
        builder.Services.AddScoped<IService<BackgroundDto, Background>, BackgroundService>();
        builder.Services.AddScoped<IService<CharacterDto, Character>, CharacterService>();
        builder.Services.AddScoped<IService<FeatDto, Feat>, FeatService>();
        builder.Services.AddScoped<ILevelUpService, LevelUpService>();
        builder.Services.AddScoped<IService<SpellDto, Spell>, SpellService>();
        builder.Services.AddScoped<ISpellServiceSystem, SpellServiceSystem>();

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

        var initializer = app.Services.GetRequiredService<IAppInitializer>();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DnDreams.Infrastructure.Persistence.DnDreamsDbContext>();
            //if (File.Exists(dbPath)) File.Delete(dbPath);
            Task.Run(async () => await InitializeDatabaseFromExcelAsync(app.Services, initializer));
            //_ = InitializeDatabaseFromExcelAsync(app.Services,initializer);

        }


        return app;
    }

    // Lógica conceptual que se integra en tu capa de persistencia/arranque
    public static async Task InitializeDatabaseFromExcelAsync(IServiceProvider services,IAppInitializer initializer)
    {
        //var initializer = services.GetRequiredService<IAppInitializer>();

        // Le pasamos el bloque de código nativo que MAUI sí sabe resolver
        await initializer.InitializeAsync(async () =>
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DnDreamsDbContext>();

            initializer.UpdateStatus("Preparando grimorio de SQLite...");
            await context.Database.MigrateAsync();

            // 2. Si no hay datos, leemos el Excel embebido en los Assets de MAUI
            if (!await context.ClassDefinitions.AnyAsync() && !await context.Spells.AnyAsync())
            {
                var importManager = scope.ServiceProvider.GetRequiredService<IExcelImportService>();
                using Stream excelStream = await FileSystem.OpenAppPackageFileAsync("DnDreams_v2.xlsx");

                // MAUI resuelve esto de forma nativa:
                await importManager.ImportDataFromExcelAsync(excelStream);
            }
        });
    }
}
