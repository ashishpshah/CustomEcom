using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JewelryStore.Infra
{
	public partial class UserMenuAccess : EntitiesBase
	{
		[NotMapped] public override int Id { get; set; }
		public int RoleId { get; set; }
		public int UserId { get; set; }
		public int MenuId { get; set; }
		public bool IsCreate { get; set; }
		public bool IsUpdate { get; set; }
		public bool IsRead { get; set; }
		public bool IsDelete { get; set; }

		[NotMapped] public string RoleName { get; set; } = null;
		[NotMapped] public string UserName { get; set; } = null;
		[NotMapped] public string MenuName { get; set; } = null;
		[NotMapped] public string Area { get; set; } = null;
		[NotMapped] public string Controller { get; set; } = null;
		[NotMapped] public string Url { get; set; } = null;
		[NotMapped] public int ParentMenuId { get; set; }
		[NotMapped] public string ParentMenuName { get; set; } = null;
		[NotMapped] public int? DisplayOrder { get; set; } = null;

		[NotMapped] public List<UserMenuAccess> Children { get; set; } = new();

	}

}