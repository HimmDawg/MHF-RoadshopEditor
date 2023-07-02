using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using CsvHelper;
using CsvHelper.Configuration;

using RoadshopEditor.Models;
using RoadshopEditor.Services.Mapping;

namespace RoadshopEditor.Services.Import;

public sealed class ImportRoadshopItemService : IImportRoadshopItemService
{
	public Span<RoadshopItem> ImportRoadshopItems(string filePath)
	{
		if (Path.GetExtension(filePath) != ".csv")
		{
			throw new ArgumentException("The file has to be a csv file");
		}

		var config = new CsvConfiguration(CultureInfo.InvariantCulture)
		{
			Delimiter = ",",
			HasHeaderRecord = true,
			PrepareHeaderForMatch = args => args.Header.ToLower()
		};

		List<RoadshopItem> items = new();
		using (var reader = new StreamReader(filePath))
		using (var csvReader = new CsvReader(reader, config))
		{
			csvReader.Context.RegisterClassMap<RoadshopClassMap>();
			items = csvReader.GetRecords<RoadshopItem>().ToList();
		}

		return CollectionsMarshal.AsSpan(items);
	}
}
