using System.ComponentModel.DataAnnotations.Schema;

namespace JewelryStore.Areas.Admin.Models
{
	public class Users : EntitiesBase
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string UserName { get; set; }
		public string Password { get; set; }

		public int? RoleId { get; set; }

		public string Fullname { get { return (!string.IsNullOrEmpty(FirstName) ? FirstName : "") + (!string.IsNullOrEmpty(LastName) ? " " + LastName : ""); } }
		public string RoleName { get; set; }

		public bool IsPassword_Reset { get; set; }
	}

	public class Roles : EntitiesBase
	{
		public string RoleName { get; set; }
		public string Description { get; set; }
		public string SelectedMenu { get; set; }
		public string SelectedMenu_Names { get; set; }
	}

	public class RolePermissions : EntitiesBase
	{
		public int? RoleId { get; set; }
		public int? ActionId { get; set; }
	}
}
