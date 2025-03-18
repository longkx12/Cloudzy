using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class AccountRepository : Repository<User>, IAccountRepository
    {
        public AccountRepository(DbCloudzyContext dbCloudzyContext) : base(dbCloudzyContext)
        {
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
