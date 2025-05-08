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
        private readonly IShipperService _shipperService;

        public AdminOrderDetailController(IOrderService orderService, IOrderDetailService orderDetailService, IShipperService shipperService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _shipperService = shipperService;
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
        public async Task<IActionResult> ConfirmOrder(int id, int? shipperId = null)
        {
            var result = await _orderService.UpdateOrderStatusAndShipperAsync(id, "Processing", shipperId);
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
        public async Task<JsonResult> UpdateOrderStatusAjax(int id, string status, int? shipperId = null)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAndShipperAsync(id, status, shipperId);
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

                    return Json(new
                    {
                        success = true,
                        message = message,
                        newStatus = status
                    });
                }
                else
                {
                    message = "Không thể cập nhật trạng thái đơn hàng!";
                    return Json(new
                    {
                        success = false,
                        message = message
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetShippers()
        {
            try
            {
                var shippers = await _shipperService.GetAllShippersAsync();
                return Json(shippers.Select(s => new { id = s.UserId, name = s.Fullname }).ToList());
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = ex.Message });
            }
        }
    }
}