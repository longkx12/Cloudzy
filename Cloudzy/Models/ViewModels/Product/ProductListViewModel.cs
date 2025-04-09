namespace Cloudzy.Models.ViewModels.Product
{
    public class ProductListViewModel
    {
        public int ProductId { get; set; }
        public int STT { get; set; }
        public string ProductName { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public string? Material { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public List<string> ProductImages { get; set; } = new();
    }
}
