using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IUserRepository 
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task LockUserAsync(int id, bool lockStatus);
    }
}
