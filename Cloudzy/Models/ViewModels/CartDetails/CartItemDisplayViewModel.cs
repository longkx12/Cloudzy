namespace Cloudzy.Models.ViewModels.CartDetails
{
    public class CartItemDisplayViewModel
    {
        public int CartItemId { get; set; }
        public int? VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public int Stock { get; set; }
    }
}
