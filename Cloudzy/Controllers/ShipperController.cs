using Cloudzy.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Shipper")]
    public class ShipperController : Controller
    {
        private readonly DbCloudzyContext _context;

        public ShipperController(DbCloudzyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var processingOrders = await _context.Orders
                .Where(o => o.Status == "Processing")
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(processingOrders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.ProductImages)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != "Processing")
            {
                TempData["ErrorMessage"] = "Đơn hàng này không ở trạng thái chờ xử lý";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != "Processing")
            {
                return BadRequest(new { success = false, message = "Đơn hàng không ở trạng thái chờ xử lý" });
            }

            order.Status = "Shipping";
            order.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã nhận đơn hàng thành công" });
        }

        [HttpPost]
        public async Task<IActionResult> DeliverOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != "Shipping")
            {
                return BadRequest(new { success = false, message = "Đơn hàng không ở trạng thái đang giao" });
            }

            order.Status = "Delivered";
            order.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã giao hàng thành công" });
        }
    }
}
