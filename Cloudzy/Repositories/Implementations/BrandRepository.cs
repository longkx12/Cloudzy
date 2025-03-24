using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly DbCloudzyContext _context;
        public BrandRepository(DbCloudzyContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Brand entity)
        {
            await _context.Brands.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await GetByIdAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task UpdateAsync(Brand entity)
        {
            _context.Brands.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
