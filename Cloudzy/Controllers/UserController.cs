using Cloudzy.Models.ViewModels;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class UserController : Controller
    {
        private readonly IAccountService _accountService;
        public UserController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                return role == "Admin" ? RedirectToAction("Index", "AdminUser")
                    : RedirectToAction("Home", "Home");
            }
            return View();
        }

        [HttpPost]        
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.LoginAsync(model);

            if (user == null)
            {
                TempData["ToastMessage"] = "Email hoặc mật khẩu không đúng!";
                TempData["ToastType"] = "error";
                return View(model);
            }

            return user.Role?.RoleName == "Admin" ? RedirectToAction("Index", "AdminUser") : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _accountService.RegisterAsync(model);
                if (user != null)
                {
                    TempData["ToastMessage"] = "Đăng ký thành công!";
                    TempData["ToastType"] = "success";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ToastMessage"] = "Đăng ký thất bại. Vui lòng thử lại.";
                    TempData["ToastType"] = "error";
                    return View(model);
                }
            }
            catch(Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login", "User");
        }
    }
}
