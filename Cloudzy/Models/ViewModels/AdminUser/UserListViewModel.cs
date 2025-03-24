namespace Cloudzy.Models.ViewModels.AdminUser
{
    public class UserListViewModel
    {
        public int UserId { get; set; }
        public int STT { get; set; }
        public string Email { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
