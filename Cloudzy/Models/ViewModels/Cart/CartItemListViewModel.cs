namespace Cloudzy.Models.ViewModels.Cart
{
    public class CartItemListViewModel
    {
        public int STT { get; set; }
        public int CartItemId { get; set; }
        public int? CartId { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }
        public int Quantity { get; set; }
        public decimal? TotalPrice { get; set; } 
        public DateTime AddedAt { get; set; }
    }
}
