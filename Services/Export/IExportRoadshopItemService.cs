using System.Collections.Generic;

using RoadshopEditor.Models;

namespace RoadshopEditor.Services.Export;

public interface IExportRoadshopItemService
{
	void ExportRoadshopItems(string filePath, IEnumerable<RoadshopItem> items);
}
