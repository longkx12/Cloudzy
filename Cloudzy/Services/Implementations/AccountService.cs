using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;
using Cloudzy.Repositories.Implementations;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;

namespace Cloudzy.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public AccountService(IAccountRepository accountRepository, IHttpContextAccessor httpContextAccessor)
        {
            _accountRepository = accountRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User?> LoginAsync(LoginViewModel model)
        {
            var user = await _accountRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
                return null;

            if (user.IsLocked)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (result != PasswordVerificationResult.Success)
                return null;

            if (string.IsNullOrEmpty(user.LoginProvider))
            {
                user.LoginProvider = "Local";
                await _accountRepository.UpdateUserAsync(user);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Fullname),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("LoginProvider", user.LoginProvider ?? "Local")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
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
                RoleId = 2,
                IsLocked = false,
                LoginProvider = "Local"
            };

            user.Password = _passwordHasher.HashPassword(user, model.Password);
            await _accountRepository.AddUserAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Fullname),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
                new Claim("LoginProvider", user.LoginProvider)
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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _accountRepository.GetUserByEmailAsync(email);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _accountRepository.UpdateUserAsync(user);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            var user = await _accountRepository.GetUserByEmailAsync(toEmail);
            var fullName = user?.Fullname ?? "";

            var fromAddress = new MailAddress("cloudzyshopvn@gmail.com", "Cloudzy");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "gpetipqrfuljksjq";
            const string subject = "Đặt lại mật khẩu Cloudzy Shop";
            string body = $@"
                <p>Xin chào <strong>{fullName}</strong>,</p>
                <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu từ bạn cho tài khoản Cloudzy Shop.</p>
                <p>Vui lòng nhấp vào nút dưới đây để tạo mật khẩu mới cho tài khoản:</p>
                <p>
                    <a href='{resetLink}'>Đặt lại mật khẩu</a>
                </p>
                <p>Nếu bạn không yêu cầu đặt lại mật khẩu, bạn có thể bỏ qua email này.</p>
                <p>Trân trọng,<br/>
                Cloudzy Shop</p>";

            using (var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            })
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(message);
            }
        }

        public async Task<bool> IsUserLockedAsync(string email)
        {
            var user = await _accountRepository.GetUserByEmailAsync(email);
            return user?.IsLocked ?? false;
        }

        public async Task<User> AddGoogleUserAsync(User user)
        {
            var existingUser = await _accountRepository.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                if (string.IsNullOrEmpty(existingUser.LoginProvider))
                {
                    existingUser.LoginProvider = "Google";
                    await _accountRepository.UpdateUserAsync(existingUser);
                }
                return existingUser;
            }

            user.LoginProvider = "Google";
            await _accountRepository.AddUserAsync(user);

            return await _accountRepository.GetUserByEmailAsync(user.Email);
        }
    }
}