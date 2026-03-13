namespace JewelryStore.Areas.Admin.Models
{
	public class Attribute : EntitiesBase
	{
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string Values { get; set; }
		public string Values_Ids { get; set; }
	}
}
