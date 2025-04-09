using Cloudzy.Data;
using Cloudzy.Models.ViewModels.AdminImportDetail;
using Cloudzy.Services.Implementations;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminImportDetailController : Controller
    {
        private readonly IImportDetailService _service;
        private readonly DbCloudzyContext _context;
        public AdminImportDetailController(IImportDetailService service,DbCloudzyContext context)
        {
            _service = service;
            _context = context;
        }
        public IActionResult Index(int importId)
        {
            ViewBag.ImportId = importId;
            return View();
        }

        public async Task<IActionResult> Load(int? page, int importId)
        {
            var pageSize = 5;
            var pageNumber = page ?? 1;

            var importDetails = await _service.GetAllAsync(importId, pageNumber, pageSize);
            if (importDetails==null || !importDetails.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_ImportDetailListPartial", importDetails);
        }

        public IActionResult Create(int importId)
        {
            var model = new CreateViewModel
            {
                ImportId = importId,
                Product = new SelectList(_context.Products, "ProductId", "ProductName")
            };
            ViewBag.ImportId = importId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Product = new SelectList(_context.Products, "ProductId", "ProductName");
                ViewBag.ImportId = model.ImportId;
                return View(model);
            }
            try
            {
                await _service.AddAsync(model);
                TempData["ToastMessage"] = "Thêm thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index", new { importId = model.ImportId });
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
                model.Product = new SelectList(_context.Products, "ProductId", "ProductName");
                ViewBag.ImportId = model.ImportId;
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var detail = await _service.GetByIdAsync(id);
            if (detail == null) return NotFound();

            detail.Product = new SelectList(_context.Products, "ProductId", "ProductName", detail.ProductId);
            ViewBag.ImportId = detail.ImportId;
            return View(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            // Lấy dữ liệu dropdown
            var products = new SelectList(_context.Products, "ProductId", "ProductName", model.ProductId);
            if (!ModelState.IsValid)
            {
                model.Product = products;
                return View(model);
            }

            try
            {
                await _service.UpdateAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index", new { importId = model.ImportId });
            }
            catch (Exception ex)
            {
                model.Product = products;
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
                // Lấy importId của Detail trước khi xóa
                var detail = await _context.ImportDetails.FindAsync(id);
                int importId = detail?.ImportId ?? 0;

                await _service.DeleteAsync(id);
                TempData["ToastMessage"] = "Xóa thành công!";
                TempData["ToastType"] = "success";

                return RedirectToAction("Index", new { importId });
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Lỗi khi xóa: " + ex.Message;
                TempData["ToastType"] = "error";

                return RedirectToAction("Index", new { importId = _context.ImportDetails.Find(id)?.ImportId ?? 0 });
            }
        }
    }
}
