using System.ComponentModel.DataAnnotations.Schema;

namespace RoadshopEditor.Models
{
	public sealed class RoadshopItem
	{
		[Column("id")]
		public int ItemHash { get; set; }

		[Column("shop_type")]
		public int? ShopType { get; set; }

		[Column("shop_id")]
		public ShopCategory ShopId { get; set; }

		[Column("item_id")]
		public int ItemId { get; set; }

		[Column("cost")]
		public int? PointCost { get; set; }

		[Column("quantity")]
		public ushort? TradeQuantity { get; set; }

		[Column("min_hr")]
		public ushort? LowRankRequirement { get; set; }

		[Column("min_sr")]
		public ushort? HighRankRequirement { get; set; }

		[Column("min_gr")]
		public ushort? GRankRequirement { get; set; }

		[Column("store_level")]
		public ushort? StoreLevelRequirement { get; set; }

		[Column("max_quantity")]
		public ushort? MaxQuantity { get; set; }

		[Column("road_floors")]
		public ushort? HuntingRoadLevelRequirement { get; set; }

		[Column("road_fatalis")]
		public ushort? WeeklyFatalisKills { get; set; }

		[NotMapped]
		public string Name { get; set; } = string.Empty;
	}
}
