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
    }
}
