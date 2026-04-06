using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryStore.Areas.Admin.Models
{
	public class	Customer : EntitiesBase
	{
        public int Id { get; set; }

		public int CustomerId { get; set; }

		public string? FirstName { get; set; }
		public string? LastName { get; set; }

		public string? Email { get; set; }
		public string? MobileNo { get; set; }

		public string? Password { get; set; }
		public bool IsPassword_Reset { get; set; }

		public DateTime? DateOfBirth { get; set; }
		public string? Gender { get; set; }

		public string FullName => $"{FirstName} {LastName}".Trim();


		public string? AddressLine1 { get; set; }
		public string? AddressLine2 { get; set; }

		public string? City { get; set; }
		public string? State { get; set; }

		public string? PostalCode { get; set; }
		public string? Country { get; set; }
		public string? AddressType { get; set; }

        public string? AlternativeMobileNo { get; set; } 
        public string? CustomerName { get; set; } 

        [NotMapped] public List<CustomerAddress>? AddressList { get; set; }
		[NotMapped]public List<CustomerCart>? CartList { get; set; }

	}
}
