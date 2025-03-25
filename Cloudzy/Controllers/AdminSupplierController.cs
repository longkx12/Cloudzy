using Cloudzy.Models.ViewModels.AdminSupplier;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class AdminSupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        public AdminSupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Load(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var suppliers = await _supplierService.GetAllAsync(pageNumber, pageSize);

            if (suppliers == null || !suppliers.Any())
            {
                return NotFound("Không có dữ liệu.");
            }

            return PartialView("_SupplierListPartial", suppliers);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SupplierCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.AddAsync(model);
                TempData["SuccessMessage"] = "Thêm nhà cung cấp thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _supplierService.GetByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SupplierEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _supplierService.UpdateAsync(model);
                TempData["SuccessMessage"] = "Cập nhật nhà cung cấp thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _supplierService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa nhà cung cấp thành công!";
            return RedirectToAction("Index");
        }
    }
}
