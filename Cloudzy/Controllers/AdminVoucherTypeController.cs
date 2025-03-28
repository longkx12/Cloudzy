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
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _voucherTypeService.AddAsync(model);
                TempData["ToastMessage"] = "Thêm thành công!";
                TempData["ToastType"] = "success";
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
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _voucherTypeService.UpdateAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _voucherTypeService.DeleteAsync(id);
                TempData["ToastMessage"] = "Xóa thành công!";
                TempData["ToastType"] = "success";
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Không thể xóa vì có voucher sử dụng loại này!";
                TempData["ToastType"] = "error";
            }
            return RedirectToAction("Index");
        }
    }
}
