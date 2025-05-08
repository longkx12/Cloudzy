namespace Cloudzy.Models.ViewModels.AdminImportDetail
{
    public class ListViewModel
    {
        public int STT { get; set; }
        public int ImportDetailId { get; set; }
        public int? ImportId { get; set; }
        public string? ProductName { get; set; }
        public string? SizeName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}