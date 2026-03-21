namespace JewelryStore.Areas.Api.DTO
{
    public class ProductPagingRequest
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Search { get; set; }
        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;
        public int SortColumnIndex { get; set; } = 0;
        public string SortDirection { get; set; } = "DESC";
    }
}
