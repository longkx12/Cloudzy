namespace Cloudzy.Models.ViewModels.SalesReport
{
    public class SalesReportViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public decimal AverageOrderValue { get; set; }
        public SalesReportFilterViewModel AppliedFilters { get; set; } = new SalesReportFilterViewModel();
        public List<SalesReportItemViewModel> SalesDetails { get; set; } = new List<SalesReportItemViewModel>();
        public List<CategorySalesViewModel> CategorySales { get; set; } = new List<CategorySalesViewModel>();
        public List<TimePeriodSalesViewModel> TimePeriodSales { get; set; } = new List<TimePeriodSalesViewModel>();
    }
}
