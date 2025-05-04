namespace Cloudzy.Models.ViewModels.SalesReport
{
    public class SalesReportItemViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public decimal OrderTotal { get; set; }
        public string PaymentMethod { get; set; }
        public int ProductCount { get; set; }
        public string Category { get; set; }
        public int? CategoryId { get; set; }
    }
}
