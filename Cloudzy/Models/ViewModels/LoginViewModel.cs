using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email không được để trống")]
        [EmailAddress(ErrorMessage ="Email không hợp lệ")]
        [StringLength(255,ErrorMessage ="Email quá dài!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải lớn hơn 8")]
        [StringLength(20, ErrorMessage = "Mật khẩu không được vượt quá 20 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
