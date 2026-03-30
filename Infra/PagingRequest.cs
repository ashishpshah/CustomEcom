namespace JewelryStore.Infra
{
    public class PagingRequest
    {
        public string Search { get; set; }
        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;
        public int SortColumnIndex { get; set; } = 0;
        public string SortDirection { get; set; } = "DESC";
    }
    public class ReviewPagingRequest
    {
        
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Search { get; set; }
        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;
        public int SortColumnIndex { get; set; } = 0;
        public string SortDirection { get; set; } = "DESC";
    }
    public class JsonParameters
    {
       
        public int ProductId { get; set; } = 0;
        public int VariantId { get; set; } = 10;
       
    }
}
