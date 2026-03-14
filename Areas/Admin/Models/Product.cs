namespace JewelryStore.Areas.Admin.Models
{
	public class Product : EntitiesBase
	{
		public int? CategoryId { get; set; }
		public string ProductName { get; set; }
		public string CategoryName { get; set; }
		public string ProductDescription { get; set; }
		public string SKU { get; set; }
		public string ImagePath { get; set; }
		public decimal? Price { get; set; }
		public List<ProductImages> ProductImages { get; set; }
		public List<ProductVariantMapping> ProductVariantMapping { get; set; }
	}

	public class ProductImages : EntitiesBase
	{
		public int? ProductId { get; set; }
		public int? VariantId { get; set; }
		public string ImagePath { get; set; }
		public bool IsPrimary { get; set; }
		public bool IsRemove { get; set; }
	}

	public class ProductVariantMapping : EntitiesBase
	{
		public int? ProductId { get; set; }
		public string SKU { get; set; }
		public decimal? Price { get; set; }
		public int? StockQuantity { get; set; }
		public string ImagePath { get; set; }

		public string VariantAttributes { get; set; }
		public int AvailableStock { get; set; }
		public int ReservedStock { get; set; }

		public int TotalStock { get; set; }

		public List<ProductVariantDetails> ProductVariantDetails { get; set; }
	}

	public class ProductVariantDetails : EntitiesBase
	{
		public int? VariantId { get; set; }
		public int? AttributeId { get; set; }
		public string AttributeName { get; set; }
		public int? AttributeValueId { get; set; }
		public string AttributeValueName { get; set; }
	}
}
