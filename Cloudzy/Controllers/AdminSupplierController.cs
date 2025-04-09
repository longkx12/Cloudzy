using Cloudzy.Models.ViewModels.AdminSupplier;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        public AdminSupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }
        public IActionResult Index()
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
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _supplierService.AddAsync(model);
                    TempData["ToastMessage"] = "Thêm thành công!";
                    TempData["ToastType"] = "success";
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    TempData["ToastMessage"] = ex.Message;
                    TempData["ToastType"] = "error";
                }
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
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _supplierService.UpdateAsync(model);
                    TempData["ToastMessage"] = "Cập nhật thành công!";
                    TempData["ToastType"] = "success";
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    TempData["ToastMessage"] = ex.Message;
                    TempData["ToastType"] = "error";
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _supplierService.DeleteAsync(id);
            TempData["ToastMessage"] = "Xóa thành công!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Index");
        }
    }
}
