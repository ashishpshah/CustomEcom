namespace JewelryStore.Areas.Admin.Models
{
	public class Inquiries : EntitiesBase
	{
		public int? UserId { get; set; }
		public int? ProductId { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public string Status { get; set; }
	}

	public class Reviews : EntitiesBase
	{
		public int? ProductId { get; set; }
		public int? UserId { get; set; }

		public int? Rating { get; set; }
		public string ReviewText { get; set; }
		public bool? IsApproved { get; set; }
	}
}
