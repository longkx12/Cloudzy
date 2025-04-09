using Cloudzy.Data;
using Cloudzy.Models.ViewModels.Cart;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Controllers
{
    public class CartItemController : Controller
    {
        private readonly ICartItemService _service;
        private readonly DbCloudzyContext _context;
        public CartItemController(ICartItemService service, DbCloudzyContext context)
        {
            _service = service;
            _context = context;
        }
        public IActionResult Index(int cartId)
        {
            ViewBag.CartId = cartId;
            return View();
        }

        public async Task<IActionResult> Load(int? page, int cartId)
        {
            var pageSize = 5;
            var pageNumber = page ?? 1;

            var cartItem = await _service.GetAllAsync(cartId, pageNumber, pageSize);
            if (cartItem == null || !cartItem.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_CartItemListPartial", cartItem);
        }

        public IActionResult Create(int cartId)
        {
            var model = new CartItemCreateViewModel
            {
                CartId = cartId
            };
            ViewBag.CartId = cartId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CartItemCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CartId = model.CartId;
                return View(model);
            }
            try
            {
                await _service.AddAsync(model);
                TempData["ToastMessage"] = "Thêm thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index", new { cartId = model.CartId });
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
                ViewBag.CartId = model.CartId;
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cartItem = await _service.GetByIdAsync(id);
            if (cartItem == null) return NotFound();

            ViewBag.CartId = cartItem.CartId;
            return View(cartItem);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CartItemEditViewModel model)
        {
            try
            {
                await _service.UpdateAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index", new { cartId = model.CartId });
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Lấy id của Cart trước khi xóa
                var cartItem = await _context.CartItems.FindAsync(id);
                int cartId = cartItem?.CartId ?? 0;

                await _service.DeleteAsync(id);
                TempData["ToastMessage"] = "Xóa thành công!";
                TempData["ToastType"] = "success";

                return RedirectToAction("Index", new { cartId });
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Lỗi khi xóa: " + ex.Message;
                TempData["ToastType"] = "error";

                return RedirectToAction("Index", new { cartId = _context.CartItems.Find(id)?.CartItemId ?? 0 });
            }
        }
    }
}
