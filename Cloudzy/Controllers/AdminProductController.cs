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
            var model = new ProductCreateViewModel
            {
                Category = new SelectList(_context.Categories,"CategoryId","CategoryName"),
                Brand = new SelectList(_context.Brands,"BrandId","BrandName")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName");
                model.Brand = new SelectList(_context.Brands, "BrandId", "BrandName");
                return View(model);
            }

            await _productService.AddAsync(model);
            TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
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
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Category = new SelectList(_context.Categories, "CategoryId", "CategoryName", model.CategoryId);
                model.Brand = new SelectList(_context.Brands, "BrandId", "BrandName", model.BrandId);
                return View(model);
            }

            await _productService.UpdateAsync(model);
            TempData["SuccessMessage"] = "Sửa sản phẩm thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            return RedirectToAction("Index");
        }
    }
}
