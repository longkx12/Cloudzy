using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Models.ViewModels.AdminDiscountCode
{
    public class DiscountCodeEditViewModel
    {
        public int DiscountCodeId { get; set; }
        public string Code { get; set; } = null!;
        public int VoucherTypeId { get; set; }
        public int Quantity { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public IEnumerable<SelectListItem> VoucherTypes { get; set; } = new List<SelectListItem>();
    }
}
