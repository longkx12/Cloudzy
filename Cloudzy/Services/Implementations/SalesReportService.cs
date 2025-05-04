using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.SalesReport;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;

namespace Cloudzy.Services.Implementations
{
    public class SalesReportService : ISalesReportService
    {
        private readonly ISalesReportRepository _salesReportRepository;

        public SalesReportService(ISalesReportRepository salesReportRepository)
        {
            _salesReportRepository = salesReportRepository;
        }

        public async Task<SalesReportViewModel> GenerateReportAsync(SalesReportFilterViewModel filter)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                startDate = filter.StartDate.Value;
                endDate = filter.EndDate.Value;
            }
            else
            {
                var now = DateTime.Now;
                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = now;
            }

            var orders = await _salesReportRepository.GetDeliveredOrdersAsync(startDate, endDate, null);

            var report = new SalesReportViewModel
            {
                AppliedFilters = filter
            };

            if (!orders.Any())
            {
                return report;
            }

            var orderIds = orders.Select(o => o.OrderId).ToList();
            var orderDetailsDict = await _salesReportRepository.GetOrderDetailsAsync(orderIds);

            var userIds = orders.Where(o => o.UserId.HasValue).Select(o => o.UserId.Value).Distinct().ToList();
            var customerNames = await _salesReportRepository.GetCustomerNamesAsync(userIds);

            var allOrderDetails = orderDetailsDict.Values.SelectMany(details => details).ToList();
            var variantIds = allOrderDetails.Where(od => od.VariantId.HasValue).Select(od => od.VariantId.Value).Distinct().ToList();

            report.TotalRevenue = orders.Sum(o => o.TotalPrice);
            report.TotalOrders = orders.Count;
            report.TotalProducts = allOrderDetails.Sum(od => od.Quantity);
            report.AverageOrderValue = report.TotalOrders > 0 ? report.TotalRevenue / report.TotalOrders : 0;

            var salesDetails = new List<SalesReportItemViewModel>();
            var categoryRevenueDict = new Dictionary<int, decimal>();
            var categoryProductCountDict = new Dictionary<int, int>();
            var categoryOrderCountDict = new Dictionary<int, int>();
            var dateRevenueDict = new Dictionary<DateTime, decimal>();
            var dateOrderCountDict = new Dictionary<DateTime, int>();

            foreach (var order in orders)
            {
                var orderDetails = orderDetailsDict.ContainsKey(order.OrderId) ? orderDetailsDict[order.OrderId] : new List<OrderDetail>();

                var categorySet = new HashSet<int>();
                int? primaryCategoryId = null;
                string primaryCategory = "Chưa phân loại";

                foreach (var detail in orderDetails)
                {
                    if (detail.Variant?.Product?.CategoryId.HasValue == true)
                    {
                        var catId = detail.Variant.Product.CategoryId.Value;
                        categorySet.Add(catId);

                        if (primaryCategoryId == null)
                        {
                            primaryCategoryId = catId;
                            primaryCategory = detail.Variant.Product.Category?.CategoryName ?? "Chưa phân loại";
                        }

                        if (!categoryRevenueDict.ContainsKey(catId))
                        {
                            categoryRevenueDict[catId] = 0;
                            categoryProductCountDict[catId] = 0;
                            categoryOrderCountDict[catId] = 0;
                        }

                        decimal itemRevenue = detail.Price * detail.Quantity;
                        categoryRevenueDict[catId] += itemRevenue;
                        categoryProductCountDict[catId] += detail.Quantity;
                        categoryOrderCountDict[catId]++;
                    }
                }

                var orderDate = order.CreatedAt.Date;
                if (!dateRevenueDict.ContainsKey(orderDate))
                {
                    dateRevenueDict[orderDate] = 0;
                    dateOrderCountDict[orderDate] = 0;
                }
                dateRevenueDict[orderDate] += order.TotalPrice;
                dateOrderCountDict[orderDate]++;

                var salesDetail = new SalesReportItemViewModel
                {
                    OrderId = order.OrderId,
                    OrderDate = order.CreatedAt,
                    CustomerName = order.UserId.HasValue && customerNames.ContainsKey(order.UserId.Value) ?
                        customerNames[order.UserId.Value] : "Unknown",
                    OrderTotal = order.TotalPrice,
                    PaymentMethod = order.PaymentMethod,
                    ProductCount = orderDetails.Sum(od => od.Quantity),
                    Category = primaryCategory,
                    CategoryId = primaryCategoryId
                };

                salesDetails.Add(salesDetail);
            }

            report.SalesDetails = salesDetails;

            if (categoryRevenueDict.Any())
            {
                var categoryIds = categoryRevenueDict.Keys.ToList();
                var categoryNames = await _salesReportRepository.GetCategoryNamesAsync(categoryIds);

                var categorySales = new List<CategorySalesViewModel>();
                foreach (var catId in categoryIds)
                {
                    categorySales.Add(new CategorySalesViewModel
                    {
                        CategoryId = catId,
                        CategoryName = categoryNames.ContainsKey(catId) ? categoryNames[catId] : "Unknown",
                        TotalRevenue = categoryRevenueDict[catId],
                        TotalProducts = categoryProductCountDict[catId],
                        TotalOrders = categoryOrderCountDict[catId],
                        Percentage = report.TotalRevenue > 0 ?
                            Math.Round((categoryRevenueDict[catId] / report.TotalRevenue) * 100, 2) : 0
                    });
                }

                report.CategorySales = categorySales.OrderByDescending(c => c.TotalRevenue).ToList();
            }

            if (dateRevenueDict.Any())
            {
                var timePeriodSales = new List<TimePeriodSalesViewModel>();
                foreach (var dateEntry in dateRevenueDict.OrderBy(d => d.Key))
                {
                    timePeriodSales.Add(new TimePeriodSalesViewModel
                    {
                        Date = dateEntry.Key,
                        Period = dateEntry.Key.ToString("yyyy-MM-dd"),
                        Revenue = dateEntry.Value,
                        OrderCount = dateOrderCountDict[dateEntry.Key]
                    });
                }

                report.TimePeriodSales = timePeriodSales;
            }

            return report;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _salesReportRepository.GetAllCategoriesAsync();
        }

        public async Task<SalesReportViewModel> GetCustomDateRangeSalesReportAsync(DateTime startDate, DateTime endDate, int? categoryId = null)
        {
            var filter = new SalesReportFilterViewModel
            {
                StartDate = startDate,
                EndDate = endDate
            };

            return await GenerateReportAsync(filter);
        }
    }
}