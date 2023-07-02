using System.Collections.Generic;

using Avalonia.Controls;
using Avalonia.Interactivity;

using RoadshopEditor.ViewModels;

namespace RoadshopEditor.Views
{
	public partial class MainWindow : Window
	{
		private readonly List<FileDialogFilter> _csvFileFilters = new()
		{
			new FileDialogFilter
			{
				Extensions = new List<string> { "csv" }
			}
		};

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void ExportItems_OnClick(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog { Filters = _csvFileFilters };
			var file = await dialog.ShowAsync(this);

			if (file is null)
			{
				return;
			}

			(DataContext as MainWindowViewModel)?.Export(file);
		}

		private async void ImportItems_OnClick(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				AllowMultiple = false,
				Filters = _csvFileFilters
			};
			var files = await dialog.ShowAsync(this);

			if (files is null)
			{
				return;
			}

			(DataContext as MainWindowViewModel)?.Import(files[0]);
		}
	}
}
