using Cloudzy.Models.ViewModels.AdminCategory;
using Cloudzy.Models.ViewModels.AdminUser;
using Cloudzy.Services.Implementations;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public AdminCategoryController (ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadUsers(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var categories = await _categoryService.GetAllAsync(pageNumber, pageSize);

            if (categories == null || !categories.Any())
            {
                return NotFound("Không có dữ liệu.");
            }

            return PartialView("_CategoryListPartial", categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddAsync(model);
                TempData["Message"] = "Thêm danh mục thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _categoryService.GetByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(model);
                TempData["Message"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            TempData["Message"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }
    }
}
