namespace Cloudzy.Models.ViewModels.SalesReport
{
    public class CategorySalesViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal Percentage { get; set; }
    }
}
