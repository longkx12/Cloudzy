namespace Cloudzy.Models.ViewModels.CartDetails
{
    public class CartDetailsViewModel
    {
        public int CartId { get; set; }
        public List<CartItemDisplayViewModel> CartItems { get; set; } = new List<CartItemDisplayViewModel>();
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string? VoucherCode { get; set; }
        public int VoucherTypeId { get; set; }
        public decimal VoucherDiscount { get; set; }
        public int ShippingMethodId { get; set; } = 2;

    }
}
