using System.ComponentModel.DataAnnotations.Schema;

namespace RoadshopEditor.Models
{
	public sealed class RoadshopItem
	{
		[Column("itemhash")]
		public int ItemHash { get; set; }

		[Column("shoptype")]
		public int? ShopType { get; set; }

		[Column("shopid")]
		public ShopCategory ShopId { get; set; }

		[Column("itemid")]
		public int ItemId { get; set; }

		[Column("points")]
		public ushort? PointCost { get; set; }

		[Column("tradequantity")]
		public ushort? TradeQuantity { get; set; }

		[Column("rankreqlow")]
		public ushort? LowRankRequirement { get; set; }

		[Column("rankreqhigh")]
		public ushort? HighRankRequirement { get; set; }

		[Column("rankreqg")]
		public ushort? GRankRequirement { get; set; }

		[Column("storelevelreq")]
		public ushort? StoreLevelRequirement { get; set; }

		[Column("maximumquantity")]
		public ushort? MaxQuantity { get; set; }

		[Column("boughtquantity")]
		public ushort? QuantityBought { get; set; }

		[Column("roadfloorsrequired")]
		public ushort? HuntingRoadLevelRequirement { get; set; }

		[Column("weeklyfataliskills")]
		public ushort? WeeklyFatalisKills { get; set; }

		[NotMapped]
		public string Name { get; set; } = string.Empty;
	}
}
