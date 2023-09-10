using Microsoft.Extensions.Logging;
using RoverCopilot.Data.Database;
using RoverCopilot.Data.Services;

namespace RoverCopilot;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
				fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemiBold");
            });

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<IMaintenanceService, MaintenanceService>();
        builder.Services.AddSingleton<DiscoveryDatabase>();


        return builder.Build();
	}
}
