using Cloudzy.Models.ViewModels.UserProfile;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileService _service;
        public UserProfileController(IUserProfileService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Profile", new { id = userId });
            }
            return RedirectToAction("Login", "User");
        }

        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var model = await _service.GetByIdAsync(userId);
                if (model == null) return NotFound();
                return View(model);
            }
            return RedirectToAction("Login", "User");
        }

        public async Task<IActionResult> Edit()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var model = await _service.GetByIdAsync(userId);
                if (model == null) return NotFound();
                return View(model);
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.UpdateAsync(model);
                    TempData["ToastMessage"] = "Cập nhật thông tin thành công!";
                    TempData["ToastType"] = "success";
                    return RedirectToAction("Profile");
                }
                catch (Exception ex)
                {
                    TempData["ToastMessage"] = ex.Message;
                    TempData["ToastType"] = "error";
                }
            }
            return View("Edit", model);
        }

        public IActionResult ChangePassword()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var model = new ChangePasswordViewModel { UserId = userId };
                return View(model);
            }
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                model.UserId = userId;
                try
                {
                    var result = await _service.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
                    if (result)
                    {
                        TempData["ToastMessage"] = "Đổi mật khẩu thành công!";
                        TempData["ToastType"] = "success";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        TempData["ToastMessage"] = "Mật khẩu hiện tại không chính xác!";
                        TempData["ToastType"] = "error";
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    TempData["ToastMessage"] = ex.Message;
                    TempData["ToastType"] = "error";
                    return View(model);
                }
            }
            return RedirectToAction("Login", "User");
        }
    }
}
