using System;

using RoadshopEditor.Models;

namespace RoadshopEditor.Services.Import;

public interface IImportRoadshopItemService
{
	Span<RoadshopItem> ImportRoadshopItems(string filePath);
}
