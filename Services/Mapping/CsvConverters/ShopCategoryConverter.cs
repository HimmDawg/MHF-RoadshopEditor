using System;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

using RoadshopEditor.Models;

namespace RoadshopEditor.Services.Mapping.CsvConverters;

internal class ShopCategoryConverter : DefaultTypeConverter
{
	public override string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
	{
		if (value is null)
		{
			return string.Empty;
		}

		if (Enum.TryParse(value.ToString(), out ShopCategory result))
		{
			return ((int)result).ToString();
		}

		return string.Empty;
	}

	public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
	{
		if (text is null)
		{
			return (ShopCategory)0;
		}

		var isValidId = int.TryParse(text, out int result);

		if (isValidId)
		{
			return (ShopCategory)result;
		}

		return (ShopCategory)0;
	}
}
