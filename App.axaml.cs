using System;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RoadshopEditor.Data;
using RoadshopEditor.Services.Export;
using RoadshopEditor.Services.Import;
using RoadshopEditor.ViewModels;
using RoadshopEditor.Views;

namespace RoadshopEditor
{
	public partial class App : Application
	{
		private IServiceProvider _services = null!;

		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			IConfiguration _configurationRoot = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", false, false)
				.Build();

			_services = new ServiceCollection()
				.AddDbContext<RoadshopContext>()
				.AddTransient<IImportRoadshopItemService, ImportRoadshopItemService>()
				.AddTransient<IExportRoadshopItemService, ExportRoadshopItemService>()
				.AddSingleton(_configurationRoot)
				.AddSingleton<MainWindowViewModel>()
				.BuildServiceProvider();

			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow
				{
					DataContext = _services.GetRequiredService<MainWindowViewModel>()
				};

				desktop.ShutdownRequested += (s, e) =>
				{
					var context = _services.GetRequiredService<RoadshopContext>();

					if (context is not null)
					{
						context.SaveChanges();
					}
				};
			}

			base.OnFrameworkInitializationCompleted();
		}
	}
}
