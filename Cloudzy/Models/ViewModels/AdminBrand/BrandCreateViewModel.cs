namespace Cloudzy.Models.ViewModels.AdminBrand
{
    public class BrandCreateViewModel
    {
        public string BrandName { get; set; } = null!;
        public IFormFile? BrandImg { get; set; }
        public string? Description { get; set; }
    }
}
