namespace JewelryStore.Areas.Admin.Models
{
	public class HomePageComponent
	{
		public long Id { get; set; }
		public string Key { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public int DisplayOrder { get; set; }
		public bool IsActive { get; set; }
		public int ItemId { get; set; }
		public string RefId { get; set; }

		public List<HomePageComponentItem> Items { get; set; }
	}

	public class HomePageComponentItem
	{
		public long Id { get; set; }
		public long ComponentId { get; set; }

		public string Title { get; set; }
		public string Subtitle { get; set; }
		public string Description { get; set; }

		public string ImagePath { get; set; }
		public string RedirectUrl { get; set; }

		public string RefId { get; set; }
		public string RefType { get; set; }

		public int DisplayOrder { get; set; }
		public bool IsActive { get; set; }
	}
}
