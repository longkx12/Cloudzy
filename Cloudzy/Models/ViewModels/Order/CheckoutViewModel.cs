namespace Cloudzy.Models.ViewModels.Order
{
    public class CheckoutViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public string VoucherCode { get; set; } = null;
        public int VoucherTypeId { get; set; } = 0;
        public decimal VoucherDiscount { get; set; } = 0;
        public decimal Total { get; set; }
        public int ShippingMethodId { get; set; }
        public List<UserAddressViewModel> Addresses { get; set; }
    }
}
