using Cloudzy.Models.ViewModels.AdminUser;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IUserService
    {
        Task<IPagedList<ListViewModel>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<EditViewModel?> GetUserByIdAsync(int id);
        Task AddUserAsync(CreateViewModel model);
        Task UpdateUserAsync(EditViewModel model);
        Task DeleteUserAsync(int id);
    }
}
