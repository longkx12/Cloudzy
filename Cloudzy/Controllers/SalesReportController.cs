using Cloudzy.Models.ViewModels.SalesReport;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalesReportController : Controller
    {
        private readonly ISalesReportService _salesReportService;

        public SalesReportController(ISalesReportService salesReportService)
        {
            _salesReportService = salesReportService;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var filter = new SalesReportFilterViewModel
            {
                StartDate = new DateTime(now.Year, now.Month, 1),
                EndDate = now,
                Categories = await _salesReportService.GetCategoriesAsync()
            };

            var report = await _salesReportService.GenerateReportAsync(filter);

            report.AppliedFilters = filter;

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(DateTime? StartDate, DateTime? EndDate)
        {
            var filter = new SalesReportFilterViewModel
            {
                StartDate = StartDate,
                EndDate = EndDate,
                Categories = await _salesReportService.GetCategoriesAsync()
            };

            if (ModelState.IsValid)
            {
                var report = await _salesReportService.GenerateReportAsync(filter);

                report.AppliedFilters = filter;

                return View("Index", report);
            }

            return View("Index", new SalesReportViewModel { AppliedFilters = filter });
        }

        public async Task<IActionResult> CustomRange(DateTime startDate, DateTime endDate)
        {
            var filter = new SalesReportFilterViewModel
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var report = await _salesReportService.GenerateReportAsync(filter);

            filter.Categories = await _salesReportService.GetCategoriesAsync();
            report.AppliedFilters = filter;

            return View("Index", report);
        }

        public async Task<IActionResult> ExportExcel(DateTime? startDate, DateTime? endDate)
        {
            var filter = new SalesReportFilterViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Categories = await _salesReportService.GetCategoriesAsync()
            };

            var report = await _salesReportService.GenerateReportAsync(filter);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Sales Report");

            worksheet.Cells[1, 1].Value = "Mã đơn hàng";
            worksheet.Cells[1, 2].Value = "Ngày đặt hàng";
            worksheet.Cells[1, 3].Value = "Người đặt hàng";
            worksheet.Cells[1, 4].Value = "Tổng tiền";
            worksheet.Cells[1, 5].Value = "Phương thức thanh toán";
            worksheet.Cells[1, 6].Value = "Số lượng";
            worksheet.Cells[1, 7].Value = "Danh mục";

            int row = 2;
            foreach (var item in report.SalesDetails)
            {
                worksheet.Cells[row, 1].Value = item.OrderId;
                worksheet.Cells[row, 2].Value = item.OrderDate.ToString("dd-MM-yyyy");
                worksheet.Cells[row, 3].Value = item.CustomerName;
                worksheet.Cells[row, 4].Value = item.OrderTotal;
                worksheet.Cells[row, 5].Value = item.PaymentMethod;
                worksheet.Cells[row, 6].Value = item.ProductCount;
                worksheet.Cells[row, 7].Value = item.Category;
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"sales-report-{DateTime.Now:ddMMyyyy}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}