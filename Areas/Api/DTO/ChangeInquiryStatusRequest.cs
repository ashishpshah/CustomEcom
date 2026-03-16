namespace JewelryStore.Areas.Api.DTO
{
    public class ChangeInquiryStatusRequest
    {
        public int InquiryId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}
