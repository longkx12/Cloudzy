using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Models.ViewModels.AdminImport
{
    public class EditViewModel
    {
        public int ImportId { get; set; }
        public int? SupplierId { get; set; }
        public DateTime ImportDate { get; set; }
        public IEnumerable<SelectListItem> Supplier { get; set; } = new List<SelectListItem>();
    }
}
