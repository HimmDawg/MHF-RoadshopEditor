using System;
using System.ComponentModel;
using System.Reflection;

namespace RoadshopEditor.Extensions;

public static class EnumExtensions
{
	public static string GetDescription(this Enum value)
	{
		FieldInfo? fi = value.GetType().GetField(value.ToString());

		if (fi is null)
		{
			return string.Empty;
		}

		DescriptionAttribute[]? attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

		if (attributes is not null && attributes.Length > 0)
		{
			return attributes[0].Description;
		}

		return value.ToString();
	}
}
