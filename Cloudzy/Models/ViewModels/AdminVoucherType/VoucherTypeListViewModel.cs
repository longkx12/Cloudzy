namespace Cloudzy.Models.ViewModels.AdminVoucherType
{
    public class VoucherTypeListViewModel
    {
        public int VoucherTypeId { get; set; }
        public int STT { get; set; }
        public string VoucherTypeName { get; set; } = null!;
        public decimal Value { get; set; }
        public decimal MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
    }
}
