using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminUser;

namespace Cloudzy.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserListViewModel>> GetAllUsersAsync();
        Task<UserEditViewModel?> GetUserByIdAsync(int id);
        Task AddUserAsync(UserCreateViewModel model);
        Task UpdateUserAsync(UserEditViewModel model);
        Task DeleteUserAsync(int id);
    }
}
