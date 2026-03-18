namespace JewelryStore.Areas.Admin.Models
{
	public class CustomerCart : EntitiesBase
	{
		public int CustomerId { get; set; }
		public int ProductId { get; set; }
		public int VariantId { get; set; }
        public string AttributeId { get; set; }
        public string AttributeName { get; set; }
        public int Quantity { get; set; }
		public string ProductName { get; set; }
		public string SKU { get; set; }
		public decimal Price { get; set; }

	}
}
