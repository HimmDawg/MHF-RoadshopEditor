using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.EntityFrameworkCore;

using RoadshopEditor.Data;
using RoadshopEditor.Models;
using RoadshopEditor.Services.Export;
using RoadshopEditor.Services.Import;

namespace RoadshopEditor.ViewModels
{
	public partial class MainWindowViewModel : ViewModelBase
	{
		[ObservableProperty] private bool _isItemPanelVisible = true;
		[ObservableProperty] private string _errorMessage = string.Empty;
		[ObservableProperty] private string _cost = string.Empty;
		[ObservableProperty] private string _qRankRequirement = string.Empty;
		[ObservableProperty] private string _quantity = string.Empty;
		[ObservableProperty] private string _maxQuantity = string.Empty;
		[ObservableProperty] private string _boughtQuantity = string.Empty;
		[ObservableProperty] private string _floorRequirement = string.Empty;
		[ObservableProperty] private string _weeklyFatalisKills = string.Empty;
		[ObservableProperty] private ShopCategory _selectedCategory = ShopCategory.BasicItems;
		[ObservableProperty] private ShopCategory[] _shopCategories = Array.Empty<ShopCategory>();
		[ObservableProperty] private Dictionary<int, string> _allItems = null!;
		[ObservableProperty] private ObservableCollection<RoadshopItem> _roadshopItems = null!;

		[ObservableProperty,
		NotifyCanExecuteChangedFor(
			nameof(AddItemCommand))]
		private KeyValuePair<int, string>? _selectedRoadshopItemName;

		[ObservableProperty,
		NotifyCanExecuteChangedFor(
			nameof(DeleteItemCommand))]
		private RoadshopItem? _selectedRoadshopItem = null!;

		[ObservableProperty,
		NotifyCanExecuteChangedFor(
			nameof(ExportCommand),
			nameof(ImportCommand),
			nameof(StartAddItemCommand),
			nameof(CancelAddItemCommand))]
		private bool _isConnectionEstablished = true;

		private readonly RoadshopContext _context;
		private readonly IImportRoadshopItemService _importService;
		private readonly IExportRoadshopItemService _exportService;

		public MainWindowViewModel(
			RoadshopContext context,
			IImportRoadshopItemService importService,
			IExportRoadshopItemService exportService)
		{
			_context = context;
			_importService = importService;
			_exportService = exportService;

			// fire and forget. Doesn't really matter here. UI will be notified anyway
			Initialize(_context).ConfigureAwait(false);
			ShopCategories = Enum.GetValues<ShopCategory>();
			IsItemPanelVisible = false;
			SelectedCategory = ShopCategories[0];
		}

		[RelayCommand(CanExecute = nameof(CanExecuteAddItem))]
		public async Task AddItem()
		{
			// need this because of non-auto-increment id
			int maxItemHash = _context.RoadshopItems.Max(i => i.ItemHash);

			// wont be null due to can execute check
			var selectedRoadshopItemKvp = SelectedRoadshopItemName!.Value;

			RoadshopItem item = new()
			{
				Name = selectedRoadshopItemKvp.Value,
				ItemHash = maxItemHash + 1,
				ShopType = 10,
				LowRankRequirement = 0,
				HighRankRequirement = 0,
				StoreLevelRequirement = 1,
				ItemId = selectedRoadshopItemKvp.Key,
				ShopId = SelectedCategory,
				PointCost = ushort.TryParse(Cost, out ushort cost)
					? cost
					: (ushort)0,
				GRankRequirement = ushort.TryParse(QRankRequirement, out ushort qRank)
					? qRank
					: (ushort)0,
				TradeQuantity = ushort.TryParse(Quantity, out ushort quantity)
					? quantity
					: (ushort)0,
				MaxQuantity = ushort.TryParse(MaxQuantity, out ushort maxQuantity)
					? maxQuantity
					: (ushort)0,
				QuantityBought = ushort.TryParse(BoughtQuantity, out ushort boughtQuantity)
					? boughtQuantity
					: (ushort)0,
				HuntingRoadLevelRequirement = ushort.TryParse(FloorRequirement, out ushort storeLevelRequirement)
					? storeLevelRequirement
					: (ushort)0,
				WeeklyFatalisKills = ushort.TryParse(WeeklyFatalisKills, out ushort weeklyFatalisKills)
					? weeklyFatalisKills
					: (ushort)0
			};

			_context.Add(item);
			RoadshopItems.Add(item);
			await _context.SaveChangesAsync();
		}

		[RelayCommand(CanExecute = nameof(CanExecuteDeleteItem))]
		public async Task DeleteItem()
		{
			if (SelectedRoadshopItem is null)
			{
				return;
			}

			_context.Remove(SelectedRoadshopItem);
			RoadshopItems.Remove(SelectedRoadshopItem);
			await _context.SaveChangesAsync();
		}

		[RelayCommand(CanExecute = nameof(CanExecuteCommands))]
		public void Export(string filePath)
		{
			_exportService.ExportRoadshopItems(filePath, RoadshopItems.ToList());
		}

		[RelayCommand(CanExecute = nameof(CanExecuteCommands))]
		public void Import(string filePath)
		{
			var items = _importService.ImportRoadshopItems(filePath);
			RoadshopItems.Clear();
			foreach (var item in items)
			{
				item.Name = AllItems[item.ItemId];
				RoadshopItems.Add(item);
			}
		}

		[RelayCommand(CanExecute = nameof(CanExecuteCommands))]
		public void StartAddItem()
		{
			IsItemPanelVisible = true;
		}

		[RelayCommand(CanExecute = nameof(CanExecuteCommands))]
		public void CancelAddItem()
		{
			IsItemPanelVisible = false;
		}

		private bool CanExecuteCommands()
		{
			return IsConnectionEstablished;
		}

		private bool CanExecuteAddItem()
		{
			return IsConnectionEstablished && SelectedRoadshopItemName is not null;
		}

		private bool CanExecuteDeleteItem()
		{
			return IsConnectionEstablished && SelectedRoadshopItem is not null;
		}

		public async Task Initialize(RoadshopContext context)
		{
			if (!await context.Database.CanConnectAsync())
			{
				ErrorMessage = "Connection to database could not be established.";
				IsConnectionEstablished = false;
				return;
			}

			if (!File.Exists("./data/mhf_items.json"))
			{
				ErrorMessage = "Could not find data/mhf_items.json. Please make sure it exists.";
				IsConnectionEstablished = false;
				return;
			}

			var roadItems = await context.RoadshopItems.ToListAsync();
			var nameDictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, string>>(File.OpenRead("./data/mhf_items.json"));
			var itemNames = new Dictionary<int, string>();

			foreach ((string itemIdHex, string name) in nameDictionary!)
			{
				// switch first two with second two characters
				var correctedHex = new string(new char[] { itemIdHex[2], itemIdHex[3], itemIdHex[0], itemIdHex[1] });

				if (!int.TryParse(correctedHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int itemid))
				{
					continue;
				}

				itemNames.Add(itemid, name);
			}

			foreach (var item in roadItems)
			{
				if (itemNames.TryGetValue(item.ItemId, out string? itemName))
				{
					item.Name = itemName ?? "";
				}
			}

			AllItems = itemNames;
			RoadshopItems = new ObservableCollection<RoadshopItem>(roadItems.OrderBy(r => r.ItemHash));
			IsConnectionEstablished = true;
		}
	}
}
