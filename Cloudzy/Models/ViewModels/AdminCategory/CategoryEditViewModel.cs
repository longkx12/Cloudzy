namespace Cloudzy.Models.ViewModels.AdminCategory
{
    public class CategoryEditViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? Description { get; set; }
    }
}
