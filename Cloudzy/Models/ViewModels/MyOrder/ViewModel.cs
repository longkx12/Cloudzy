using Cloudzy.Models.ViewModels.Order;

namespace Cloudzy.Models.ViewModels.MyOrder
{
    public class ViewModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? DiscountCode { get; set; }
        public List<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();
    }
}
