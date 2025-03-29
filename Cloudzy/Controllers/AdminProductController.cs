using Cloudzy.Data;
using Cloudzy.Models.ViewModels.AdminProduct;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly DbCloudzyContext _context;
        public AdminProductController(IProductService productService, DbCloudzyContext context)
        {
            _productService = productService;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Load(int? page)
        {
            var pageSize = 5;
            var pageNumber = page ?? 1;

            var products = await _productService.GetAllAsync(pageNumber, pageSize);
            if (products == null || !products.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_ProductListPartial", products);
        }

        public IActionResult Create()
        {
            var model = new CreateViewModel
            {
                Category = new SelectList(_context.Categories,"CategoryId","CategoryName"),
                Brand = new SelectList(_context.Brands,"BrandId","BrandName")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
                model.Brand = new SelectList(_context.Brands, "BrandId", "BrandName");
                return View(model);
            }

            try
            {
                await _productService.AddAsync(model);
                TempData["ToastMessage"] = "Thêm thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            product.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            product.Brand = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);


            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            // Lấy dữ liệu dropdown
            var categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", model.CategoryId);
            var brands = new SelectList(_context.Brands, "BrandId", "BrandName", model.BrandId);
            if (!ModelState.IsValid)
            {
                model.Category = categories;
                model.Brand = brands;
                return View(model);
            }

            try
            {
                await _productService.UpdateAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                model.Category = categories;
                model.Brand = brands;

                //Giữ lại danh sách ảnh nếu lỗi
                var product = await _productService.GetByIdAsync(model.ProductId);
                model.CurrentImages = product.CurrentImages;

                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["ToastMessage"] = "Xóa thành công!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Index");
        }
    }
}
