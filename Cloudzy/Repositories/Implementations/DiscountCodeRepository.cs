using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class DiscountCodeRepository : IDiscountCodeRepository
    {
        private readonly DbCloudzyContext _context;
        public DiscountCodeRepository(DbCloudzyContext context)
        {
            _context = context;
        }
        public async Task AddAsync(DiscountCode entity)
        {
            await _context.DiscountCodes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {

            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode != null)
            {
                _context.DiscountCodes.Remove(discountCode);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DiscountCode>> GetAllAsync()
        {
            return await _context.DiscountCodes.Include(d=>d.VoucherType).ToListAsync();
        }

        public async Task<DiscountCode> GetByIdAsync(int id)
        {
            return await _context.DiscountCodes.FindAsync(id);
        }

        public async Task UpdateAsync(DiscountCode entity)
        {
            _context.DiscountCodes.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
