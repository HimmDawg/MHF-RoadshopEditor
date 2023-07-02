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
		[ObservableProperty] private bool _isItemPanelVisible = false;
		[ObservableProperty] private string _cost = string.Empty;
		[ObservableProperty] private string _qRankRequirement = string.Empty;
		[ObservableProperty] private string _quantity = string.Empty;
		[ObservableProperty] private string _maxQuantity = string.Empty;
		[ObservableProperty] private string _boughtQuantity = string.Empty;
		[ObservableProperty] private string _floorRequirement = string.Empty;
		[ObservableProperty] private string _weeklyFatalisKills = string.Empty;
		[ObservableProperty] private ShopCategory _selectedCategory = ShopCategory.BasicItems;
		[ObservableProperty] private ShopCategory[] _shopCategories = Array.Empty<ShopCategory>();
		[ObservableProperty] private KeyValuePair<int, string> _selectedRoadshopItemName;
		[ObservableProperty] private RoadshopItem _selectedRoadshopItem = null!;
		[ObservableProperty] private Dictionary<int, string> _allItems = null!;
		[ObservableProperty] private ObservableCollection<RoadshopItem> _roadshopItems = null!;

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
			LoadItems(_context).ConfigureAwait(false);
			ShopCategories = Enum.GetValues<ShopCategory>();
			IsItemPanelVisible = false;
			SelectedCategory = ShopCategories[0];
		}

		[RelayCommand]
		public async Task AddItem()
		{
			// need this because of non-auto-increment id
			int maxItemHash = _context.RoadshopItems.Max(i => i.ItemHash);

			RoadshopItem item = new()
			{
				Name = SelectedRoadshopItemName.Value,
				ItemHash = maxItemHash + 1,
				ShopType = 10,
				LowRankRequirement = 0,
				HighRankRequirement = 0,
				StoreLevelRequirement = 1,
				ItemId = SelectedRoadshopItemName.Key,
				ShopId = SelectedCategory,
				PointCost = int.TryParse(Cost, out int cost)
					? (ushort)cost
					: (ushort)0,
				GRankRequirement = int.TryParse(QRankRequirement, out int qRank)
					? (ushort)qRank
					: (ushort)0,
				TradeQuantity = int.TryParse(Quantity, out int quantity)
					? (ushort)quantity
					: (ushort)0,
				MaxQuantity = int.TryParse(MaxQuantity, out int maxQuantity)
					? (ushort)maxQuantity
					: (ushort)0,
				QuantityBought = int.TryParse(BoughtQuantity, out int boughtQuantity)
					? (ushort)boughtQuantity
					: (ushort)0,
				HuntingRoadLevelRequirement = int.TryParse(FloorRequirement, out int storeLevelRequirement)
					? (ushort)storeLevelRequirement
					: (ushort)0,
				WeeklyFatalisKills = int.TryParse(WeeklyFatalisKills, out int weeklyFatalisKills)
					? (ushort)weeklyFatalisKills
					: (ushort)0
			};

			_context.Add(item);
			RoadshopItems.Add(item);
			await _context.SaveChangesAsync();
		}

		[RelayCommand]
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

		[RelayCommand]
		public void Export(string filePath)
		{
			_exportService.ExportRoadshopItems(filePath, RoadshopItems.ToList());
		}

		[RelayCommand]
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

		[RelayCommand]
		public void CancelAddItem()
		{
			IsItemPanelVisible = false;
		}

		[RelayCommand]
		public void StartAddItem()
		{
			IsItemPanelVisible = true;
		}

		public async Task LoadItems(RoadshopContext context)
		{
			if (!File.Exists("./data/mhf_items.json"))
			{
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
		}
	}
}
