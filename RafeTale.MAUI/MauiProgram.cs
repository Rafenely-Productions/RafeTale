using RafeTale.Application.DTOs;
using RafeTale.Application.Interfaces;
using RafeTale.Application.Interfaces.DtosInterfaces;
using RafeTale.Application.Services;
using RafeTale.Application.Services.DtosServices;
using RafeTale.Application.Services.Importer;
using RafeTale.Application.Services.Importer.Initializer;
using RafeTale.Domain.Entities;
using RafeTale.Domain.Interfaces;
using RafeTale.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace RafeTale.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        if (builder == null)
        {
            throw new InvalidOperationException("Failed to create MauiApp builder.");
        }
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "RafeTale_SRD_v1.db3");
        builder.Services.AddSingleton(dbPath);
        builder.Services.AddInfrastructure(dbPath);
        builder.Services.AddLocalization();

        var culture = new CultureInfo("es-MX");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // --- Core / Infrastructure Services ---
        builder.Services.AddSingleton<IAppInitializer, AppInitializer>();
        builder.Services.AddSingleton<IDiceService, DiceService>();
        builder.Services.AddScoped<ILocalizationService, LocalizationService>();

        // --- Domain Services (interfaces específicas) ---
        builder.Services.AddScoped<IService<ClassDefinitionDto, ClassDefinition>, ClassService>();
        builder.Services.AddScoped<IService<RaceDto, Race>, RaceService>();
        builder.Services.AddScoped<IService<SubclassDto, Subclass>, SubclassService>();
        builder.Services.AddScoped<IService<SpellDto, Spell>, SpellService>();
        builder.Services.AddScoped<IService<LanguageDto, Language>, LanguageService>();
        builder.Services.AddScoped<IService<BackgroundDto, Background>, BackgroundService>();
        builder.Services.AddScoped<IService<FeatDto, Feat>, FeatService>();
        builder.Services.AddScoped<ICharacterQueryService, CharacterQueryService>();
        builder.Services.AddScoped<IFeatureQueryService, FeatureQueryService>();
        builder.Services.AddScoped<ILevelingService, LevelingService>();
        builder.Services.AddScoped<ISpellServiceSystem, SpellServiceSystem>();
        builder.Services.AddScoped<ILevelUpService, LevelUpService>();
        builder.Services.AddScoped<ICharacterCreationService, CharacterCreationService>();

        // --- DTO Services ---
        builder.Services.AddScoped<IService<CharacterDto, Character>, CharacterService>();
        builder.Services.AddScoped<IService<SpellDto, Spell>, SpellService>();
        builder.Services.AddScoped<IService<RaceDto, Race>, RaceService>();
        builder.Services.AddScoped<IService<ClassDefinitionDto, ClassDefinition>, ClassService>();
        builder.Services.AddScoped<IService<SubclassDto, Subclass>, SubclassService>();
        builder.Services.AddScoped<IService<BackgroundDto, Background>, BackgroundService>();
        builder.Services.AddScoped<IService<FeatDto, Feat>, FeatService>();
        builder.Services.AddScoped<IService<LanguageDto, Language>, LanguageService>();
        builder.Services.AddScoped<ILibraryDataService, LibraryDataService>();
        builder.Services.AddScoped<ILibraryCountsService, LibraryCountsService>();

        //Excel
        builder.Services.AddScoped<IExcelImportService, ImportManager>();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            var logger = builder.Services.BuildServiceProvider().GetService<ILogger<App>>();
            logger?.LogCritical(ex, "Unhandled exception crashed the app");
        };
#if ANDROID
        SQLitePCL.Batteries_V2.Init();
#endif
        return builder.Build();
    }
}