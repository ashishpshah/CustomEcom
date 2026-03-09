namespace JewelryStore.Areas.Admin.Models
{
	public class Menus : EntitiesBase
	{
		public int ParentId { get; set; }

		public string Area { get; set; }
		public string Controller { get; set; }
		public string Url { get; set; }
		public string Name { get; set; }
		public string ParentMenuName { get; set; }
		public string Icon { get; set; }
		public int? DisplayOrder { get; set; }
		public string SuperAdmin { get; set; }
	}
}
