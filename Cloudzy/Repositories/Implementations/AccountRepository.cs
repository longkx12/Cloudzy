using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DbCloudzyContext _dbCloudzyContext;
        public AccountRepository(DbCloudzyContext dbCloudzyContext)
        {
            _dbCloudzyContext = dbCloudzyContext;
        }

        public async Task AddUserAsync(User user)
        {
            await _dbCloudzyContext.AddAsync(user);
            await _dbCloudzyContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _dbCloudzyContext.Users.ToListAsync();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dbCloudzyContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
