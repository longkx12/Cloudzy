using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminUser;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Controllers
{
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;
        private readonly DbCloudzyContext _context;

        public AdminUserController(IUserService userService, DbCloudzyContext context)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadUsers(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var users = await _userService.GetAllUsersAsync(pageNumber, pageSize);

            if (users == null || !users.Any())
            {
                return NotFound("Không có dữ liệu.");
            }

            return PartialView("_UserListPartial", users);
        }

        public IActionResult Create()
        {
            var model = new UserCreateViewModel
            {
                Roles = new SelectList(_context.Roles, "RoleId", "RoleName")
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.AddUserAsync(model);
                TempData["Message"] = "Thêm tài khoản thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _userService.GetUserByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateUserAsync(model);
                TempData["Message"] = "Cập nhật tài khoản thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            TempData["Message"] = "Xóa tài khoản thành công!";
            return RedirectToAction("Index");
        }
    }
}
