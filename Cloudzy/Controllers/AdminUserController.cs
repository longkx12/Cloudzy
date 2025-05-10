using Cloudzy.Data;
using Cloudzy.Models.ViewModels.AdminUser;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;
        private readonly DbCloudzyContext _context;

        public AdminUserController(IUserService userService, DbCloudzyContext context)
        {
            _userService = userService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Load(int? page)
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
            var model = new CreateViewModel
            {
                Roles = new SelectList(_context.Roles, "RoleId", "RoleName")
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.AddUserAsync(model);
                    TempData["ToastMessage"] = "Thêm thành công!";
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

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _userService.GetUserByIdAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _userService.UpdateUserAsync(model);
                TempData["ToastMessage"] = "Cập nhật thành công!";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            bool currentLockStatus = user.IsLocked;

            await _userService.LockUnlockUserAsync(id);

            bool newLockStatus = !currentLockStatus;
            string message = newLockStatus ? "Khóa tài khoản thành công" : "Mở khóa tài khoản thành công";

            return Json(new { success = true, message, isLocked = newLockStatus });
        }
    }
}