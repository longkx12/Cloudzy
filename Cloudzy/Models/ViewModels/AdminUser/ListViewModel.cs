﻿namespace Cloudzy.Models.ViewModels.AdminUser
{
    public class ListViewModel
    {
        public int UserId { get; set; }
        public int STT { get; set; }
        public string Email { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string RoleName { get; set; } = null!;
        public bool IsLocked { get; set; } = false;
    }
}
