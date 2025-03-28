namespace Cloudzy.Models.ViewModels.AdminBrand
{
    public class ListViewModel
    {
        public int BrandId { get; set; }
        public int STT { get; set; }
        public string BrandName { get; set; } = null!;
        public string? BrandImg { get; set; }
        public string? Description { get; set; }
    }
}
