using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminOrderDetail;

namespace Cloudzy.Models.ViewModels.AdminOrderDetail
{
    public class DetailViewModel
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        public string VoucherCode { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalPrice { get; set; }
        public int? ShipperId { get; set; }
        public string? ShipperName { get; set; }
        public string ReturnReason { get; set; }
        public IEnumerable<User>? AvailableShippers { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
