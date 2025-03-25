using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminBrand
{
    public class BrandEditViewModel
    {
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string BrandName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh")]
        public IFormFile? BrandImg { get; set; }
        public string? Description { get; set; }
        public string? ExistingImg { get; set; }
    }
}
