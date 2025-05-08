using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên gì mà dài thế!")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email quá dài!")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "SDT phải bắt đầu bằng 0 và có 10 chữ số")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải lớn hơn 8")]
        [StringLength(20, ErrorMessage = "Mật khẩu không được vượt quá 20 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Nhập lại mật khẩu phải lớn hơn 8")]
        [StringLength(20, ErrorMessage = "Nhập lại mật khẩu không được vượt quá 20 ký tự")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
