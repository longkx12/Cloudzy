using Cloudzy.Models.ViewModels.AdminBrand;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class AdminBrandController : Controller
    {
        private readonly IBrandService _brandService;
        public AdminBrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Load(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var brands = await _brandService.GetAllAsync(pageNumber, pageSize);

            if (brands == null || !brands.Any())
            {
                return NotFound("Không có dữ liệu.");
            }

            return PartialView("_BrandListPartial", brands);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (model.BrandImg == null || model.BrandImg.Length == 0)
            {
                ModelState.AddModelError("BrandImg", "Vui lòng tải lên một ảnh.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                await _brandService.AddAsync(model);
                TempData["ToastMessage"] = "Thêm thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _brandService.GetByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _brandService.UpdateAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _brandService.DeleteAsync(id);
            TempData["ToastMessage"] = "Xóa thành công!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Index");
        }
    }
}
