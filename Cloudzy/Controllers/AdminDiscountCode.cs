using Cloudzy.Data;
using Cloudzy.Models.ViewModels.AdminDiscountCode;
using Cloudzy.Models.ViewModels.AdminUser;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDiscountCode : Controller
    {
        private readonly IDiscountCodeService _discountCodeService;
        private readonly DbCloudzyContext _context;
        public AdminDiscountCode(IDiscountCodeService discountCodeService, DbCloudzyContext context)
        {
            _discountCodeService = discountCodeService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Load(int? page)
        {
            var pageSize = 5;
            var pageNumber = page ?? 1;

            var discountCodes = await _discountCodeService.GetAllAsync(pageNumber, pageSize);
            if(discountCodes==null || !discountCodes.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_DiscountCodeListPartial", discountCodes);
        }

        public IActionResult Create()
        {
            var model = new Models.ViewModels.AdminDiscountCode.CreateViewModel
            {
                VoucherTypes = new SelectList(_context.VoucherTypes, "VoucherTypeId", "VoucherTypeName")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Models.ViewModels.AdminDiscountCode.CreateViewModel model)
        {
            // Nếu có lỗi, cần gán lại dữ liệu dropdown
            if (!ModelState.IsValid)
            {
                model.VoucherTypes = new SelectList(_context.VoucherTypes, "VoucherTypeId", "VoucherTypeName");
                return View(model);
            }

            try
            {
                await _discountCodeService.AddAsync(model);
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
            var discountCode = await _discountCodeService.GetByIdAsync(id);
            if (discountCode == null) return NotFound();

            discountCode.VoucherTypes = new SelectList(_context.VoucherTypes, "VoucherTypeId", "VoucherTypeName", discountCode.VoucherTypeId);
            return View(discountCode);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Models.ViewModels.AdminDiscountCode.EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _discountCodeService.UpdateAsync(model);
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
            await _discountCodeService.DeleteAsync(id);
            TempData["ToastMessage"] = "Xóa thành công!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Index");
        }
    }
}
