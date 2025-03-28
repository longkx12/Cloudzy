using Cloudzy.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminUser
{
    public class EditViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(255, ErrorMessage = "Email quá dài!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "SDT phải bắt đầu bằng số 0 và có 10 số")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền")]
        public int RoleId { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
