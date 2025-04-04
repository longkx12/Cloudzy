using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Models.ViewModels.AdminImport
{
    public class CreateViewModel
    {
        public int SupplierId { get; set; }
        public DateTime ImportDate { get; set; } = DateTime.Now;
        public IEnumerable<SelectListItem> Supplier { get; set; } = new List<SelectListItem>();
    }
}
