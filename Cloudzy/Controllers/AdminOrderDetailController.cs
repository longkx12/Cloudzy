using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderDetailController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;

        public AdminOrderDetailController(IOrderService orderService, IOrderDetailService orderDetailService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var orderDetail = await _orderDetailService.GetOrderDetailByIdAsync(id);
            if (orderDetail == null)
            {
                TempData["ToastMessage"] = "Không tìm thấy đơn hàng!";
                TempData["ToastType"] = "error";
                return RedirectToAction("Index", "AdminOrder");
            }

            return View(orderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, "Processing");
            if (result)
            {
                TempData["ToastMessage"] = "Đơn hàng đã được xác nhận!";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "Không thể xác nhận đơn hàng!";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction("Index", "AdminOrder");
        }
    }
}
