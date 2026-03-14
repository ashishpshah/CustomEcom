namespace JewelryStore.Areas.Admin.Models
{
	public class ProductStock
	{
		public int SrNo { get; set; }
		public int Id { get; set; }

		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public int VariantId { get; set; }
		public string VariantName { get; set; }
		public string SKU { get; set; }
		public decimal Price { get; set; }

		public string VariantAttributes { get; set; }
		public int AvailableStock { get; set; }
		public int ReservedStock { get; set; }

		public int TotalStock { get; set; }  // Computed column
		public DateTime? LastModifiedDate { get; set; }
	}

	public class ProductStockHistory
	{
		public int SrNo { get; set; }
		public int Id { get; set; }

		public int ProductId { get; set; }
		public int VariantId { get; set; }

		public string ChangeType { get; set; } = string.Empty;

		public int Quantity { get; set; }

		public int? ReferenceId { get; set; }
		public string? ReferenceType { get; set; }

		public string? Remarks { get; set; }

		public int? LastModifiedBy { get; set; }
		public DateTime? LastModifiedDate { get; set; }
	}
}
