using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminDiscountCode
{
    public class EditViewModel
    {
        public int DiscountCodeId { get; set; }

        [Required(ErrorMessage = "Mã voucher không được để trống.")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn loại voucher.")]
        public int? VoucherTypeId { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc.")]
        public DateTime EndDate { get; set; } = DateTime.Now;
        public IEnumerable<SelectListItem> VoucherTypes { get; set; } = new List<SelectListItem>();
    }
}
