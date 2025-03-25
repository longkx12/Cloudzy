using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminCategory
{
    public class CategoryEditViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
