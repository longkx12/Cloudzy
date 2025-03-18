using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IAccountRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetAllUserAsync();
        Task<User> GetUserByEmailAsync(string email);
    }
}
