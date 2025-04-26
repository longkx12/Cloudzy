using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;

namespace Cloudzy.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
        Task<User?> RegisterAsync(RegisterViewModel model);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
        Task SendResetPasswordEmailAsync(string toEmail, string resetLink);
    }
}
