using Cloudzy.Models.ViewModels.AdminVoucherType;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class AdminVoucherTypeController : Controller
    {
        private readonly IVoucherTypeService _voucherTypeService;
        public AdminVoucherTypeController(IVoucherTypeService voucherTypeService)
        {
            _voucherTypeService = voucherTypeService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Load(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var voucherTypes = await _voucherTypeService.GetAllAsync(pageNumber, pageSize);
            if (voucherTypes == null || !voucherTypes.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_VoucherTypeListPartial", voucherTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VoucherTypeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _voucherTypeService.AddAsync(model);
                TempData["SuccessMessage"] = "Thêm loại voucher thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _voucherTypeService.GetByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VoucherTypeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _voucherTypeService.UpdateAsync(model);
                TempData["SuccessMessage"] = "Cập nhật loại voucher thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _voucherTypeService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Xóa loại voucher thành công!";
            return RedirectToAction("Index");
        }
    }
}
