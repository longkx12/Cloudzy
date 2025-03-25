using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminBrand
{
    public class BrandCreateViewModel
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string BrandName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh")]
        [DataType(DataType.Upload)]
        public IFormFile? BrandImg { get; set; }
        public string? Description { get; set; }
    }
}
