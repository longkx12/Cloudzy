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
            var model = new DiscountCodeCreateViewModel
            {
                VoucherTypes = new SelectList(_context.VoucherTypes, "VoucherTypeId", "VoucherTypeName")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiscountCodeCreateViewModel model)
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
            TempData["SuccessMessage"] = "Thêm voucher thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var discountCode = await _discountCodeService.GetByIdAsync(id);
            if (discountCode == null) return NotFound();

            return View(discountCode);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DiscountCodeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _discountCodeService.UpdateAsync(model);
                TempData["SuccessMessage"] = "Cập nhật voucher thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _discountCodeService.DeleteAsync(id);
            TempData["SuccesMessage"] = "Xóa voucher thành công!";
            return RedirectToAction("Index");
        }
    }
}
