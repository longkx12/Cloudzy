using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;
using Cloudzy.Repositories.Implementations;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Cloudzy.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor)
        {
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<User?> LoginAsync(LoginViewModel model)
        {
            var user = await _accountRepository.GetUserByEmailAsync(model.Email);

            if (user == null || user.Password != model.Password)
            {
                return null;
            }
            // Tạo claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Fullname),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("UserId",user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName??"User") 
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe, // Lưu đăng nhập nếu chọn "Remember Me"
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(1)
            };

            var httpContext = _httpContextAccessor.HttpContext;
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                  new ClaimsPrincipal(claimsIdentity),
                                  authProperties);

            return user;
        }
        public async Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        public async Task<User?> RegisterAsync(RegisterViewModel model)
        {
            var existingUser = await _accountRepository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại!");
            }

            var user = new User
            {
                Fullname = model.Fullname,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                RoleId = 2
            };
            await _accountRepository.AddUserAsync(user);

            //Đăng ký xong sẽ tự đăng nhập 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Fullname),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("UserId",user.UserId.ToString()),
                new Claim(ClaimTypes.Role,user.Role?.RoleName??"User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            var httpContext = _httpContextAccessor.HttpContext;
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return user;
        }
    }
}
