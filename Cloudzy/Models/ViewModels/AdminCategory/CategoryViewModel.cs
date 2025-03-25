using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminCategory
{
    public class CategoryViewModel
    {
        [Required(ErrorMessage = "Tên không được để trống")]
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
