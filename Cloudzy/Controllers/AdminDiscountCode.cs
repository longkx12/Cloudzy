using Cloudzy.Data;
using Cloudzy.Models.ViewModels.AdminDiscountCode;
using Cloudzy.Models.ViewModels.AdminUser;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Controllers
{
    public class AdminDiscountCode : Controller
    {
        private readonly IDiscountCodeService _discountCodeService;
        private readonly DbCloudzyContext _context;
        public AdminDiscountCode(IDiscountCodeService discountCodeService, DbCloudzyContext context)
        {
            _discountCodeService = discountCodeService;
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
            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn ngày bắt đầu");
            }

            // Nếu có lỗi, cần gán lại dữ liệu dropdown
            if (!ModelState.IsValid)
            {
                model.VoucherTypes = new SelectList(_context.VoucherTypes, "VoucherTypeId", "VoucherTypeName");
                return View(model);
            }

            await _discountCodeService.AddAsync(model);
            TempData["ToastMessage"] = "Thêm thành công!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var discountCode = await _discountCodeService.GetByIdAsync(id);
            if (discountCode == null) return NotFound();

            return View(discountCode);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Models.ViewModels.AdminDiscountCode.EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _discountCodeService.UpdateAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
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
