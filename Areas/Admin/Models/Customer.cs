using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryStore.Areas.Admin.Models
{
	public class	Customer : EntitiesBase
	{
        public int Id { get; set; }

		public int CustomerId { get; set; }

		public string FirstName { get; set; } = string.Empty;
		public string? LastName { get; set; }

		public string Email { get; set; } = string.Empty;
		public string MobileNo { get; set; } = string.Empty;

		public string? Password { get; set; }
		public bool IsPassword_Reset { get; set; }

		public DateTime? DateOfBirth { get; set; }
		public string? Gender { get; set; }

		public string FullName => $"{FirstName} {LastName}".Trim();


		public string AddressLine1 { get; set; } = string.Empty;
		public string? AddressLine2 { get; set; }

		public string City { get; set; } = string.Empty;
		public string State { get; set; } = string.Empty;

		public string PostalCode { get; set; } = string.Empty;
		public string Country { get; set; } = string.Empty;
		public string AddressType { get; set; } = string.Empty;

        [NotMapped] public List<CustomerAddress>? AddressList { get; set; }
		[NotMapped]public List<CustomerCart>? CartList { get; set; }

	}
}
