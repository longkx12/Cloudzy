namespace Cloudzy.Models.ViewModels.Order
{
    public class OrderRequestViewModel
    {
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public int ShippingMethodId { get; set; }
        public string? VoucherCode { get; set; }
        public int VoucherTypeId { get; set; }
        public decimal VoucherDiscount { get; set; }
    }
}
