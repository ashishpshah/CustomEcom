namespace JewelryStore.Areas.Admin.Models
{
	public class Orders : EntitiesBase
	{
		public int? UserId { get; set; }
		public string OrderNumber { get; set; }

		public decimal? TotalAmount { get; set; }
		public decimal? DiscountAmount { get; set; }
		public decimal? FinalAmount { get; set; }

		public string OrderStatus { get; set; }
		public string PaymentStatus { get; set; }
	}

	public class OrderItems : EntitiesBase
	{
		public int? OrderId { get; set; }
		public int? VariantId { get; set; }
		public int? Quantity { get; set; }

		public decimal? UnitPrice { get; set; }
		public decimal? TotalPrice { get; set; }
	}
}
