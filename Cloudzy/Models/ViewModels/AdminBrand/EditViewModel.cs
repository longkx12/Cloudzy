using Cloudzy.Services;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminBrand
{
    public class EditViewModel
    {
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự")]
        public string BrandName { get; set; } = null!;

        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg", ".gif" })]
        public IFormFile? BrandImg { get; set; }
        public string? Description { get; set; }
        public string? ExistingImg { get; set; }
    }
}
