using Cloudzy.Data;
using Cloudzy.Models.ViewModels.Cart;
using Cloudzy.Services.Implementations;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _service;
        private readonly DbCloudzyContext _context;
        public ShoppingCartController(IShoppingCartService service, DbCloudzyContext context)
        {
            _service = service;
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

            var carts = await _service.GetAllAsync(pageNumber, pageSize);
            if (carts == null || !carts.Any())
            {
                return NotFound("Không có dữ liệu");
            }
            return PartialView("_ShoppingCartListPartial", carts);
        }

        public IActionResult Create()
        {
            var model = new ShoppingCartCreateViewModel
            {
                Users = _context.Users.Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.Fullname 
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShoppingCartCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Users = new SelectList(_context.Users, "UserId", "UserName");
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
