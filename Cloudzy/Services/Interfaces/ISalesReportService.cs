using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.SalesReport;

namespace Cloudzy.Services.Interfaces
{
    public interface ISalesReportService
    {
        Task<SalesReportViewModel> GenerateReportAsync(SalesReportFilterViewModel filter);
        Task<List<Category>> GetCategoriesAsync();
        Task<SalesReportViewModel> GetCustomDateRangeSalesReportAsync(DateTime startDate, DateTime endDate, int? categoryId = null);
    }
}