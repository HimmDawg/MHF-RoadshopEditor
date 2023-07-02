using CsvHelper.Configuration;

using RoadshopEditor.Models;

namespace RoadshopEditor.Services.Mapping;

internal sealed class RoadshopClassMap : ClassMap<RoadshopItem>
{
	internal RoadshopClassMap()
	{
		Map(m => m.ShopType).Name("shoptype");
		Map(m => m.ShopId).Name("shopid");
		Map(m => m.ItemHash).Name("itemhash");
		Map(m => m.ItemId).Name("itemid");
		Map(m => m.PointCost).Name("points");
		Map(m => m.TradeQuantity).Name("tradequantity");
		Map(m => m.LowRankRequirement).Name("rankreqlow");
		Map(m => m.HighRankRequirement).Name("rankreqhigh");
		Map(m => m.GRankRequirement).Name("rankreqg");
		Map(m => m.StoreLevelRequirement).Name("storelevelreq");
		Map(m => m.MaxQuantity).Name("maximumquantity");
		Map(m => m.QuantityBought).Name("boughtquantity");
		Map(m => m.HuntingRoadLevelRequirement).Name("roadfloorsrequired");
		Map(m => m.WeeklyFatalisKills).Name("weeklyfataliskills");
	}
}
