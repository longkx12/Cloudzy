using Cloudzy.Data;
using Cloudzy.Models.ViewModels.AdminProductVariant;
using Cloudzy.Services.Implementations;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cloudzy.Controllers
{
    public class AdminProductVariantController : Controller
    {
        private readonly IProductVariantService _productVariantService;
        private readonly DbCloudzyContext _context;
        public AdminProductVariantController(IProductVariantService productVariantService, DbCloudzyContext context)
        {
            _productVariantService = productVariantService;
            _context = context;
        }
        public async Task<IActionResult> Index(int productId)
        {
            ViewBag.ProductId = productId;
            return View();
        }

        public async Task<IActionResult> Load(int? page,int productId)
        {
            var pageSize = 5;
            var pageNumber = page ?? 1;

            var productVariants = await _productVariantService.GetAllAsync(productId, pageNumber, pageSize);
            if (productVariants == null || !productVariants.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_ProductVariantListPartial", productVariants);
        }

        public IActionResult Create(int productId)
        {
            if (!_context.Products.Any(p => p.ProductId == productId))
            {
                return NotFound("Sản phẩm không tồn tại.");
            }

            var model = new CreateViewModel
            {
                ProductId = productId,
                SizeList = new SelectList(_context.Sizes, "SizeId", "SizeName")
            };
            ViewBag.ProductId = productId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SizeList = new SelectList(_context.Sizes, "SizeId", "SizeName");
                ViewBag.ProductId = model.ProductId;
                return View(model);
            }

            try
            {
                var existingProduct = await _context.Products.FindAsync(model.ProductId);
                if (existingProduct == null)
                {
                    ModelState.AddModelError("ProductId", "Sản phẩm không tồn tại.");
                    model.SizeList = new SelectList(_context.Sizes, "SizeId", "SizeName");
                    ViewBag.ProductId = model.ProductId;
                    return View(model);
                }

                await _productVariantService.AddAsync(model);
                TempData["ToastMessage"] = "Thêm thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index", new { productId = model.ProductId });
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
                model.SizeList = new SelectList(_context.Sizes, "SizeId", "SizeName");
                ViewBag.ProductId = model.ProductId;
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var variant = await _productVariantService.GetByIdAsync(id);
            if (variant == null) return NotFound();

            variant.SizeList = new SelectList(_context.Sizes, "SizeId", "SizeName", variant.SizeId);

            return View(variant);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            // Lấy dữ liệu dropdown
            var sizes = new SelectList(_context.Sizes, "SizeId", "SizeName", model.SizeId);
            if (!ModelState.IsValid)
            {
                model.SizeList = sizes;
                return View(model);
            }

            try
            {
                // Kiểm tra xem đã tồn tại variant khác với cùng ProductId và SizeId chưa
                var existingVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.ProductId == model.ProductId &&
                                              pv.SizeId == model.SizeId &&
                                              pv.VariantId != model.VariantId);

                if (existingVariant != null)
                {
                    ModelState.AddModelError("SizeId", "Đã tồn tại size này cho sản phẩm.");
                    model.SizeList = sizes;
                    return View(model);
                }

                await _productVariantService.UpdateAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index", new { productId = model.ProductId });
            }
            catch (DbUpdateException dbEx)
            {
                Debug.WriteLine($"Database error: {dbEx.InnerException?.Message}");
                TempData["ToastMessage"] = "Lỗi cơ sở dữ liệu: " + dbEx.InnerException?.Message;
                TempData["ToastType"] = "error";
                model.SizeList = sizes;
            }
            catch (Exception ex)
            {
                model.SizeList = sizes;
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
                // Get the variant to retrieve its productId before deletion
                var variant = await _context.ProductVariants.FindAsync(id);
                int productId = variant?.ProductId ?? 0;

                await _productVariantService.DeleteAsync(id);
                TempData["ToastMessage"] = "Xóa thành công!";
                TempData["ToastType"] = "success";

                return RedirectToAction("Index", new { productId });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete error: {ex.Message}");
                TempData["ToastMessage"] = "Lỗi khi xóa: " + ex.Message;
                TempData["ToastType"] = "error";

                // Redirect back to the list
                return RedirectToAction("Index", new { productId = _context.ProductVariants.Find(id)?.ProductId ?? 0 });
            }
        }
    }
}
