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

        [HttpPost]
        public async Task<JsonResult> UpdateOrderStatusAjax(int id, string status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            var orderDetail = await _orderDetailService.GetOrderDetailByIdAsync(id);

            string message;
            if (result)
            {
                switch (status)
                {
                    case "Processing":
                        message = "Đơn hàng đã được xác nhận thành công!";
                        break;
                    case "Shipping":
                        message = "Đơn hàng đã được chuyển cho shipper!";
                        break;
                    case "Delivered":
                        message = "Đơn hàng đã được giao thành công!";
                        break;
                    case "Cancelled":
                        message = "Đơn hàng đã được hủy!";
                        break;
                    default:
                        message = $"Đơn hàng đã được cập nhật thành {status}!";
                        break;
                }
            }
            else
            {
                message = "Không thể cập nhật trạng thái đơn hàng!";
            }

            return Json(new
            {
                success = result,
                message = message,
                newStatus = status,
                orderDetail = orderDetail
            });
        }
    }
}