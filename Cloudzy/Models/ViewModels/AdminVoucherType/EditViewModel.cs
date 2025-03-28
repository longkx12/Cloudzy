using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminVoucherType
{
    public class EditViewModel
    {
        public int VoucherTypeId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string VoucherTypeName { get; set; } = null!;

        [Required(ErrorMessage = "Giá trị không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá trị phải lớn hơn 0")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "Giá trị tối thiểu không được để trống")]
        [Range(1000, double.MaxValue, ErrorMessage = "Giá tối thiểu phải lớn hơn hoặc bằng 1000")]
        public decimal MinimumValue { get; set; }

        [Required(ErrorMessage = "Giá trị tối đa không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá tối đa phải là số dương")]
        public decimal? MaximumValue { get; set; }
    }
}
