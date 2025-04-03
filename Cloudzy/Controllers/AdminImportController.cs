using Cloudzy.Data;
using Cloudzy.Models.ViewModels.AdminImport;
using Cloudzy.Services.Implementations;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Controllers
{
    public class AdminImportController : Controller
    {
        private readonly IImportService _service;
        private readonly DbCloudzyContext _context;
        public AdminImportController(IImportService service, DbCloudzyContext context)
        {
            _context = context;
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Load(int? page)
        {
            var pageSize = 5;
            var pageNumber = page ?? 1;

            var import = await _service.GetAllAsync(pageNumber, pageSize);
            if (import == null || !import.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_ImportListPartial", import);
        }

        public IActionResult Create()
        {
            var model = new CreateViewModel
            {
                Supplier = new SelectList(_context.Suppliers, "SupplierId", "SupplierName")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            // Nếu có lỗi, cần gán lại dữ liệu dropdown
            if (!ModelState.IsValid)
            {
                model.Supplier = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
                return View(model);
            }

            try
            {
                await _service.AddAsync(model);
                TempData["ToastMessage"] = "Thêm thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var import = await _service.GetByIdAsync(id);
            if (import == null) return NotFound();

            import.Supplier = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", import.SupplierId);
            return View(import);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(model);
                    TempData["ToastMessage"] = "Cập nhật thành công!";
                    TempData["ToastType"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
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
            await _service.DeleteAsync(id);
            TempData["ToastMessage"] = "Xóa thành công!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Index");
        }
    }
}
