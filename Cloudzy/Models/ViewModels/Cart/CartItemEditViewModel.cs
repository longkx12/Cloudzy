namespace Cloudzy.Models.ViewModels.Cart
{
    public class CartItemEditViewModel
    {
        public int CartItemId { get; set; }
        public int? CartId { get; set; }
        public int? VariantId { get; set; }
        public int Quantity { get; set; }
    }
}
