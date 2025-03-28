using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminSupplier
{
    public class EditViewModel
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string SupplierName { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "SDT phải bắt đầu bằng số 0 và có 10 số")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? Address { get; set; }
    }
}
