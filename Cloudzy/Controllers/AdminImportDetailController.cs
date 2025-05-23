﻿using Cloudzy.Data;
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

        public AdminImportDetailController(IImportDetailService service, DbCloudzyContext context)
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
            if (importDetails == null || !importDetails.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_ImportDetailListPartial", importDetails);
        }

        public async Task<IActionResult> Create(int importId)
        {
            var import = await _context.Imports
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(i => i.ImportId == importId);

            if (import == null)
                return NotFound("Không tìm thấy phiếu nhập hàng");

            var products = await _context.Products
                .Where(p => p.SupplierId == import.SupplierId)
                .ToListAsync();

            var model = new CreateViewModel
            {
                ImportId = importId,
                Product = new SelectList(products, "ProductId", "ProductName"),
                Sizes = new List<SelectListItem>() 
            };

            ViewBag.ImportId = importId;
            ViewBag.SupplierName = import.Supplier?.SupplierName ?? "Không xác định";

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var import = await _context.Imports
                    .FirstOrDefaultAsync(i => i.ImportId == model.ImportId);

                if (import == null)
                    return NotFound("Không tìm thấy phiếu nhập hàng");

                var products = await _context.Products
                    .Where(p => p.SupplierId == import.SupplierId)
                    .ToListAsync();

                model.Product = new SelectList(products, "ProductId", "ProductName");
                model.Sizes = await _service.GetSizesByProductIdAsync(model.ProductId);
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

                var import = await _context.Imports
                    .FirstOrDefaultAsync(i => i.ImportId == model.ImportId);

                if (import == null)
                    return NotFound("Không tìm thấy phiếu nhập hàng");

                var products = await _context.Products
                    .Where(p => p.SupplierId == import.SupplierId)
                    .ToListAsync();

                model.Product = new SelectList(products, "ProductId", "ProductName");
                model.Sizes = await _service.GetSizesByProductIdAsync(model.ProductId);
                ViewBag.ImportId = model.ImportId;
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var detail = await _service.GetByIdAsync(id);
            if (detail == null) return NotFound();

            var import = await _context.Imports
                .FirstOrDefaultAsync(i => i.ImportId == detail.ImportId);

            if (import == null)
                return NotFound("Không tìm thấy phiếu nhập hàng");

            var products = await _context.Products
                .Where(p => p.SupplierId == import.SupplierId)
                .ToListAsync();

            detail.Product = new SelectList(products, "ProductId", "ProductName", detail.ProductId);
            detail.Sizes = await _service.GetSizesByProductIdAsync(detail.ProductId);
            ViewBag.ImportId = detail.ImportId;
            return View(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var import = await _context.Imports
                    .FirstOrDefaultAsync(i => i.ImportId == model.ImportId);

                if (import == null)
                    return NotFound("Không tìm thấy phiếu nhập hàng");

                var products = await _context.Products
                    .Where(p => p.SupplierId == import.SupplierId)
                    .ToListAsync();

                model.Product = new SelectList(products, "ProductId", "ProductName", model.ProductId);
                model.Sizes = await _service.GetSizesByProductIdAsync(model.ProductId);
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
                var import = await _context.Imports
                    .FirstOrDefaultAsync(i => i.ImportId == model.ImportId);

                if (import == null)
                    return NotFound("Không tìm thấy phiếu nhập hàng");

                var products = await _context.Products
                    .Where(p => p.SupplierId == import.SupplierId)
                    .ToListAsync();

                model.Product = new SelectList(products, "ProductId", "ProductName", model.ProductId);
                model.Sizes = await _service.GetSizesByProductIdAsync(model.ProductId);
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

        [HttpGet]
        public async Task<IActionResult> GetSizesByProduct(int productId)
        {
            var sizes = await _service.GetSizesByProductIdAsync(productId);
            return Json(sizes);
        }
    }
}