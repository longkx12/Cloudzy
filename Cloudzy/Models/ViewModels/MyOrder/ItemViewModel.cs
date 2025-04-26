namespace Cloudzy.Models.ViewModels.MyOrder
{
    public class ItemViewModel
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string Size { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
