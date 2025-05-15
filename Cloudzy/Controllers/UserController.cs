using Cloudzy.Models.ViewModels;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Cloudzy.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;

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
                    : RedirectToAction("Index", "Home");
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

            var user = await _accountService.GetUserByEmailAsync(model.Email);
            if (user != null && user.IsLocked)
            {
                TempData["ToastMessage"] = "Tài khoản này đã bị khóa";
                TempData["ToastType"] = "error";
                return View(model);
            }

            user = await _accountService.LoginAsync(model);

            if (user == null)
            {
                TempData["ToastMessage"] = "Email hoặc mật khẩu không đúng!";
                TempData["ToastType"] = "error";
                return View(model);
            }

            TempData["ToastMessage"] = "Đăng nhập thành công!";
            TempData["ToastType"] = "success";

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
            catch (Exception ex)
            {
                TempData["ToastMessage"] = ex.Message;
                TempData["ToastType"] = "error";
                return View(model);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ToastMessage"] = "Email không tồn tại trong hệ thống!";
                TempData["ToastType"] = "error";
                return View();
            }

            if (user.IsLocked)
            {
                TempData["ToastMessage"] = "Tài khoản này đã bị khóa. Không thể đặt lại mật khẩu.";
                TempData["ToastType"] = "error";
                return View();
            }

            var token = Guid.NewGuid().ToString();
            user.ResetPasswordToken = token;
            user.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1); // Hạn token 1 tiếng

            await _accountService.UpdateUserAsync(user);

            var resetLink = Url.Action("ResetPassword", "User", new { token = token, email = model.Email }, Request.Scheme);

            await _accountService.SendResetPasswordEmailAsync(model.Email, resetLink);

            TempData["ToastMessage"] = "Link đặt lại mật khẩu đã được gửi tới email của bạn!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _accountService.GetUserByEmailAsync(model.Email);
            if (user == null || user.ResetPasswordToken != model.Token || user.ResetPasswordExpiry < DateTime.UtcNow)
            {
                TempData["ToastMessage"] = "Token không hợp lệ hoặc đã hết hạn!";
                TempData["ToastType"] = "error";
                return RedirectToAction("ForgotPassword");
            }

            if (user.IsLocked)
            {
                TempData["ToastMessage"] = "Tài khoản này đã bị khóa. Không thể đặt lại mật khẩu.";
                TempData["ToastType"] = "error";
                return RedirectToAction("Login");
            }

            user.Password = new PasswordHasher<User>().HashPassword(user, model.NewPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordExpiry = null;
            await _accountService.UpdateUserAsync(user);

            TempData["ToastMessage"] = "Đổi mật khẩu thành công!";
            TempData["ToastType"] = "success";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                TempData["ToastMessage"] = "Đăng nhập Google thất bại";
                TempData["ToastType"] = "error";
                return RedirectToAction("Login");
            }

            // Lấy thông tin từ Google
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _accountService.GetUserByEmailAsync(email);

            if (user == null)
            {
                var newUser = new User
                {
                    Email = email,
                    Fullname = name,
                    RoleId = 2,
                    IsLocked = false,
                    LoginProvider = "Google"
                };

                var passwordHasher = new PasswordHasher<User>();
                var randomPassword = Guid.NewGuid().ToString();
                newUser.Password = passwordHasher.HashPassword(newUser, randomPassword);

                await _accountService.AddGoogleUserAsync(newUser);

                user = newUser;
            }
            else if (user.IsLocked)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ToastMessage"] = "Tài khoản này đã bị khóa";
                TempData["ToastType"] = "error";
                return RedirectToAction("Login");
            }
            else if (string.IsNullOrEmpty(user.LoginProvider))
            {
                user.LoginProvider = "Google";
                await _accountService.UpdateUserAsync(user);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Fullname),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("LoginProvider", user.LoginProvider ?? "Google")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["ToastMessage"] = "Đăng nhập Google thành công!";
            TempData["ToastType"] = "success";

            return user.Role?.RoleName == "Admin" ? RedirectToAction("Index", "AdminUser") : RedirectToAction("Index", "Home");
        }
    }
}