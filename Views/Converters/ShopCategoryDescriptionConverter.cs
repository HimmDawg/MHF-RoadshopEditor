using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Data.Converters;

using RoadshopEditor.Extensions;
using RoadshopEditor.Models;

namespace RoadshopEditor.Views.Converters;

public sealed class ShopCategoryDescriptionConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is ShopCategory shopCategory)
		{
			return shopCategory.GetDescription();
		}
		else
		{
			return string.Empty;
		}
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
