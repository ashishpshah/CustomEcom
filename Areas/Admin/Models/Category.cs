namespace JewelryStore.Areas.Admin.Models
{
	public class Category : EntitiesBase
	{
		public string CategoryName { get; set; }
		public string ParentCategoryName { get; set; }
		public string ImagePath { get; set; }
		public int? ParentCategoryId { get; set; }
		public bool ImagePath_Remove { get; set; }
	}
}
