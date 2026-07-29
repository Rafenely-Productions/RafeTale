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

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "dndreams.db3");
        builder.Services.AddSingleton(dbPath);
        builder.Services.AddInfrastructure(dbPath);
        builder.Services.AddLocalization();

        var culture = new CultureInfo("es-MX");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        // --- Core / Infrastructure Services ---
        builder.Services.AddSingleton<IAppInitializer, AppInitializer>();
        builder.Services.AddScoped<ILocalizationService, LocalizationService>();

        // --- Domain Services (interfaces específicas) ---
        builder.Services.AddScoped<IService<ClassDefinitionDto, ClassDefinition>, ClassService>();
        builder.Services.AddScoped<IService<RaceDto, Race>, RaceService>();
        builder.Services.AddScoped<IService<SubclassDto, Subclass>, SubclassService>();
        builder.Services.AddScoped<IService<SpellDto, Spell>, SpellService>();
        builder.Services.AddScoped<IService<LanguageDto, Language>, LanguageService>();
        builder.Services.AddScoped<IService<BackgroundDto, Background>, BackgroundService>();
        builder.Services.AddScoped<IService<CharacterDto, Character>, CharacterService>();
        builder.Services.AddScoped<IService<FeatDto, Feat>, FeatService>();
        builder.Services.AddScoped<ICharacterQueryService, CharacterQueryService>();
        builder.Services.AddScoped<IFeatureQueryService, FeatureQueryService>();
        builder.Services.AddScoped<ILevelingService, LevelingService>();
        builder.Services.AddScoped<ISpellServiceSystem, SpellServiceSystem>();
        builder.Services.AddScoped<ILevelUpService, LevelUpService>();

        // --- DTO Services ---
        builder.Services.AddScoped<IService<CharacterDto, Character>, CharacterService>();
        builder.Services.AddScoped<IService<SpellDto, Spell>, SpellService>();
        builder.Services.AddScoped<IService<RaceDto, Race>, RaceService>();
        builder.Services.AddScoped<IService<ClassDefinitionDto, ClassDefinition>, ClassService>();
        builder.Services.AddScoped<IService<SubclassDto, Subclass>, SubclassService>();
        builder.Services.AddScoped<IService<BackgroundDto, Background>, BackgroundService>();
        builder.Services.AddScoped<IService<FeatDto, Feat>, FeatService>();
        builder.Services.AddScoped<IService<LanguageDto, Language>, LanguageService>();

        //Excel
        builder.Services.AddScoped<IExcelImportService, ImportManager>();

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            var logger = builder.Services.BuildServiceProvider().GetService<ILogger<App>>();
            logger?.LogCritical(ex, "Unhandled exception crashed the app");
        };

        return builder.Build();
    }
}