using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email quá dài!")]
        public string Email { get; set; }
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
