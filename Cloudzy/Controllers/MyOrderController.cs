using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    [Authorize]
    public class MyOrderController : Controller
    {
        private readonly IMyOrderService _service;

        public MyOrderController(IMyOrderService service)
        {
            _service = service;
        }

        public async Task<IActionResult> MyOrders(string status = "all")
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var model = await _service.GetOrdersByUserIdAsync(userId, status);
                ViewBag.CurrentStatus = status;
                return View(model);
            }
            return RedirectToAction("Login", "User");
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var order = await _service.GetOrderDetailByIdAsync(id, userId);
                if (order == null) return NotFound();
                return View(order);
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var result = await _service.CancelOrderAsync(id, userId);
                if (result)
                {
                    TempData["ToastMessage"] = "Đơn hàng đã được hủy thành công!";
                    TempData["ToastType"] = "success";
                }
                else
                {
                    TempData["ToastMessage"] = "Không thể hủy đơn hàng này!";
                    TempData["ToastType"] = "error";
                }
                return RedirectToAction("OrderDetail", new { id });
            }
            return RedirectToAction("Login", "User");
        }
    }
}
