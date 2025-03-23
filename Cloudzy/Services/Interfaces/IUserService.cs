using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminUser;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IUserService
    {
        Task<IPagedList<UserListViewModel>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<UserEditViewModel?> GetUserByIdAsync(int id);
        Task AddUserAsync(UserCreateViewModel model);
        Task UpdateUserAsync(UserEditViewModel model);
        Task DeleteUserAsync(int id);
    }
}
