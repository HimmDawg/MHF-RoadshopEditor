using System.Collections.Generic;
using System.Globalization;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;

using RoadshopEditor.Models;
using RoadshopEditor.Services.Mapping;

namespace RoadshopEditor.Services.Export;

public sealed class ExportRoadshopItemService : IExportRoadshopItemService
{
	public void ExportRoadshopItems(string filePath, IEnumerable<RoadshopItem> items)
	{
		var config = new CsvConfiguration(CultureInfo.InvariantCulture)
		{
			Delimiter = ",",
			HasHeaderRecord = true,
			PrepareHeaderForMatch = args => args.Header.ToLower()
		};

		using var csvReader = new CsvWriter(new StreamWriter(filePath), config);
		csvReader.Context.RegisterClassMap<RoadshopClassMap>();
		csvReader.WriteRecords(items);
	}
}
