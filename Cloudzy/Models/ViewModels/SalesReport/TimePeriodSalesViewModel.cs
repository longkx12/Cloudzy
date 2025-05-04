namespace Cloudzy.Models.ViewModels.SalesReport
{
    public class TimePeriodSalesViewModel
    {
        public DateTime Date { get; set; }
        public string Period { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }
}
