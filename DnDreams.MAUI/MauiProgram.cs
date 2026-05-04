using Microsoft.Extensions.Logging;
using DnDreams.Infrastructure;
using DnDreams.Application.Services;

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
#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
