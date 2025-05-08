using Cloudzy.Models.Domain;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Shipper")]
    public class ShipperController : Controller
    {
        private readonly IShipperService _shipperService;

        public ShipperController(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        public async Task<IActionResult> Index()
        {
            var processingOrders = await _shipperService.GetProcessingOrdersAsync();

            ViewData["ActiveTab"] = "processing";
            return View(processingOrders);
        }

        public async Task<IActionResult> Shipping()
        {
            var shipperId = int.Parse(User.FindFirstValue("UserId"));

            var shippingOrders = await _shipperService.GetShippingOrdersByShipperIdAsync(shipperId);

            ViewData["ActiveTab"] = "shipping";
            return View("Index", shippingOrders);
        }

        public async Task<IActionResult> Delivered()
        {
            var shipperId = int.Parse(User.FindFirstValue("UserId"));

            var deliveredOrders = await _shipperService.GetDeliveredOrdersByShipperIdAsync(shipperId);

            ViewData["ActiveTab"] = "delivered";
            return View("Index", deliveredOrders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _shipperService.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            // Check if the order can be viewed by the current shipper
            if (order.Status == "Shipping" || order.Status == "Delivered")
            {
                var shipperId = int.Parse(User.FindFirstValue("UserId"));
                if (order.ShipperId != shipperId)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem đơn hàng này";
                    return RedirectToAction(nameof(Index));
                }
            }
            else if (order.Status != "Processing")
            {
                TempData["ErrorMessage"] = "Đơn hàng này không ở trạng thái phù hợp để xem";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var shipperId = int.Parse(User.FindFirstValue("UserId"));

            var result = await _shipperService.AcceptOrderAsync(id, shipperId);

            if (!result)
            {
                return BadRequest(new { success = false, message = "Đơn hàng không ở trạng thái chờ xử lý" });
            }

            return Json(new { success = true, message = "Đã nhận đơn hàng thành công" });
        }

        [HttpPost]
        public async Task<IActionResult> DeliverOrder(int id)
        {
            var shipperId = int.Parse(User.FindFirstValue("UserId"));

            var result = await _shipperService.DeliverOrderAsync(id, shipperId);

            if (!result)
            {
                return BadRequest(new { success = false, message = "Đơn hàng không ở trạng thái đang giao hoặc không thuộc về bạn" });
            }

            return Json(new { success = true, message = "Đã giao hàng thành công" });
        }
    }
}