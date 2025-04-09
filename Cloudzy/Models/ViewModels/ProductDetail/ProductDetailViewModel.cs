namespace Cloudzy.Models.ViewModels.ProductDetail
{
    public class ProductDetailViewModel
    {
        public int VariantId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string SizeName { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public string? Material { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? HeightRange { get; set; }
        public string? WeightRange { get; set; }
        public int Stock { get; set; }
        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
