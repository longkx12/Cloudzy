using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải lớn hơn 8")]
        [StringLength(20, ErrorMessage = "Mật khẩu không được vượt quá 20 ký tự")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Nhập lại mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Nhập lại mật khẩu phải lớn hơn 8")]
        [StringLength(20, ErrorMessage = "Nhập lại mật khẩu không được vượt quá 20 ký tự")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage ="Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
    }
}
