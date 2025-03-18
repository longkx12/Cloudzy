using Cloudzy.Data;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbCloudzyContext _dbCloudzyContext;
        protected readonly DbSet<T> _dbSet;
        public Repository(DbCloudzyContext dbCloudzyContext)
        {
            _dbCloudzyContext = dbCloudzyContext;
            _dbSet = dbCloudzyContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByEmailAsync(string email)
        {
            return await _dbSet.FindAsync(email);
        }
    }
}
