using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Models.ViewModels.AdminImportDetail
{
    public class EditViewModel
    {
        public int ImportDetailId { get; set; }
        public int ImportId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public IEnumerable<SelectListItem> Product { get; set; } = new List<SelectListItem>();
    }
}
