namespace Cloudzy.Models.ViewModels.AdminBrand
{
    public class BrandEditViewModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public IFormFile? BrandImg { get; set; }
        public string? Description { get; set; }
        public string? ExistingImg { get; set; }
    }
}
