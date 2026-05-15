using DnDreams.Application.Interfaces;
using DnDreams.Application.Services;
using DnDreams.Infrastructure;
using DnDreams.Infrastructure.Services;
using Microsoft.Extensions.Logging;

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

		builder.Services.AddInfrastructure(dbPath);//.AddInfrastructure(dbPath);
		builder.Services.AddSingleton<LevelingService>();

		builder.Services.AddScoped<IExcelImportService, ImportManager>();
        builder.Services.AddScoped<ICharacterQueryService, CharacterQueryService>();
        builder.Services.AddScoped<IFeatureQueryService, FeatureQueryService>();
        builder.Services.AddScoped<ILevelingService, LevelingService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DnDreams.Infrastructure.Persistence.DnDreamsDbContext>();
            //if (File.Exists(dbPath)) File.Delete(dbPath);
            context.Database.EnsureCreated();
        }

        return app;
	}
}
