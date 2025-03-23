using Cloudzy.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminUser
{
    public class UserEditViewModel
    {
        public int UserId { get; set; }


        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string Fullname { get; set; } = null!;


        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quyền")]
        public int RoleId { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}
