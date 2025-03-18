using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;

namespace Cloudzy.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }
}
