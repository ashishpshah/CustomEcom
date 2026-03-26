namespace JewelryStore.Areas.Api.DTO
{
    public class CartRequest
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public int? Quantity { get; set; }
    }
    public class OfferRequest
    {
        public int CustomerId { get; set; }
        public string CouponCode { get; set; }
        public int SelectedOfferId { get; set; }
        public int RemoveOfferId { get; set; }
    }
}
