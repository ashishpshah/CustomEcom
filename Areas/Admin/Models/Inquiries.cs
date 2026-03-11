namespace JewelryStore.Areas.Admin.Models
{
	public class Inquiries : EntitiesBase
	{
		public int? InquiryId { get; set; }
		public int? UserId { get; set; }
		public int? ProductId { get; set; }
		public string ProductName { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public string Status { get; set; }
        public string Status_Desc { get; set; }
        public DateTime? Inquiry_Date { get; set; }
        public string OldStatus_Desc { get; set; }
        public string NewStatus_Desc { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string Remarks { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string Inquiry_By { get; set; }
        public string ReplyBy { get; set; }
        public string ReplyBy_Text { get; set; }
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
