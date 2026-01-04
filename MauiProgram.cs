using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MyMauiApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>();

		builder.Services.AddMauiBlazorWebView();
		builder.Logging.AddDebug();

		return builder.Build();
	}
}
