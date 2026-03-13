namespace JewelryStore.Areas.Admin.Models
{
	public class CustomerAddress
	{
		public int Id { get; set; }

		public int CustomerId { get; set; }

		public string AddressLine1 { get; set; } = string.Empty;
		public string? AddressLine2 { get; set; }

		public string City { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;

		public string PostalCode { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;

		public bool IsDefault { get; set; }

		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }

		public int CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }

		public int? LastModifiedBy { get; set; }
		public DateTime? LastModifiedDate { get; set; }
	}
}
