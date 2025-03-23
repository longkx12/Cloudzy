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

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbCloudzyContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _dbCloudzyContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _dbCloudzyContext.SaveChangesAsync();
        }

        //public async Task<T> GetByEmailAsync(string email)
        //{
        //    return await _dbSet.FindAsync(email);
        //}
    }
}
