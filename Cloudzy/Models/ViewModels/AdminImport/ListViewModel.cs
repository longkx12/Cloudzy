namespace Cloudzy.Models.ViewModels.AdminImport
{
    public class ListViewModel
    {
        public int ImportId { get; set; }
        public int STT { get; set; }
        public string SupplierName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime ImportDate { get; set; }
    }
}
