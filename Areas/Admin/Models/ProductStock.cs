namespace JewelryStore.Areas.Admin.Models
{
	public class ProductStock
	{
		public int Id { get; set; }

		public int ProductId { get; set; }
		public int VariantId { get; set; }

		public int AvailableStock { get; set; }
		public int ReservedStock { get; set; }

		public int TotalStock { get; set; }  // Computed column
	}

	public class ProductStockHistory
	{
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
