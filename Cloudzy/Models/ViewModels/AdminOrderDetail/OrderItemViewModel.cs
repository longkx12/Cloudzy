namespace Cloudzy.Models.ViewModels.AdminOrderDetail
{
    public class OrderItemViewModel
    {
        public int OrderDetailId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string SizeName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }
}
