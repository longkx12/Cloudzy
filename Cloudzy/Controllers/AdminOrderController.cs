using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _service;

        public AdminOrderController(IOrderService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Danh sách đơn hàng";
            return View();
        }

        public IActionResult ReturnRequests()
        {
            ViewData["Title"] = "Yêu cầu hoàn trả hàng";
            return View();
        }

        public async Task<IActionResult> Load(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            var orders = await _service.GetAllAsync(pageNumber, pageSize);

            if (orders == null || !orders.Any())
            {
                return NotFound("Không có dữ liệu.");
            }

            return PartialView("_OrderListPartial", orders);
        }

        public async Task<IActionResult> LoadReturnRequests(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;
            var returnOrders = await _service.GetReturnRequestsAsync(pageNumber, pageSize);

            if (returnOrders == null || !returnOrders.Any())
            {
                return NotFound("Không có yêu cầu hoàn trả nào.");
            }

            return PartialView("_ReturnRequestsPartial", returnOrders);
        }
    }
}