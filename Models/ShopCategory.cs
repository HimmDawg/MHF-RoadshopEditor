using System.ComponentModel;

namespace RoadshopEditor.Models;

public enum ShopCategory
{
	[Description("Basic Items")] BasicItems,
	[Description("Gatheriables")] Gatherables,
	[Description("HR 1-4")] LowHR,
	[Description("HR 5-7")] HighHR,
	[Description("GR 1+")] GR,
	[Description("Decorations")] Decorations,
	[Description("Other Items")] OtherItems,
	[Description("Weekly Limited ")] WeeklyLimited,
	[Description("Super Special")] SuperSpecial
}
