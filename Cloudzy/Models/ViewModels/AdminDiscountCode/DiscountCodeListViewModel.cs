namespace Cloudzy.Models.ViewModels.AdminDiscountCode
{
    public class DiscountCodeListViewModel
    {
        public int DiscountCodeId { get; set; }
        public int STT { get; set; }
        public string Code { get; set; } = null!;
        public string VoucherTypeName { get; set; } = null!;
        public int Quantity { get; set; }
        public int UsedQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
