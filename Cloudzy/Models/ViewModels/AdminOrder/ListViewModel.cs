namespace Cloudzy.Models.ViewModels.AdminOrder
{
    public class ListViewModel
    {
        public int STT { get; set; }
        public int OrderId { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? VoucherCode { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
