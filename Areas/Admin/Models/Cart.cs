namespace JewelryStore.Areas.Admin.Models
{
	public class Cart : EntitiesBase
	{
		public int? UserId { get; set; }
		public int? VariantId { get; set; }
		public int? Quantity { get; set; }
	}

	public class Coupons : EntitiesBase
	{
		public string CouponCode { get; set; }
		public string DiscountType { get; set; }
		public decimal? DiscountValue { get; set; }
		public decimal? MinimumOrderAmount { get; set; }
		public DateTime? ExpiryDate { get; set; }
		public int? UsageLimit { get; set; }
	}
}
