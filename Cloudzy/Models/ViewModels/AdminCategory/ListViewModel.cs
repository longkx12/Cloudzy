namespace Cloudzy.Models.ViewModels.AdminCategory
{
    public class ListViewModel
    {
        public int CategoryId { get; set; }
        public int STT { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
    }
}
