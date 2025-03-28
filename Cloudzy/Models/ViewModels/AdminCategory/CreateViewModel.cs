using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminCategory
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(50, ErrorMessage = "Tên danh mục không được quá 50 ký tự")]
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
