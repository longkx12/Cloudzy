using Cloudzy.Models.Domain;
using Cloudzy.Services;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminBrand
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string BrandName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh")]
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg", ".gif" })]
        public IFormFile? BrandImg { get; set; }
        public string? Description { get; set; }
    }
}