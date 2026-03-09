namespace JewelryStore.Areas.Admin.Models
{
	public class Attribute : EntitiesBase
	{
		public string AttributeName { get; set; }
	}

	public class AttributeValue : EntitiesBase
	{
		public int? AttributeId { get; set; }
		public string Value { get; set; }
	}
}
