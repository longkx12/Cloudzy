namespace Cloudzy.Models.ViewModels.AdminSupplier
{
    public class ListViewModel
    {
        public int SupplierId { get; set; }
        public int STT { get; set; }
        public string SupplierName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
