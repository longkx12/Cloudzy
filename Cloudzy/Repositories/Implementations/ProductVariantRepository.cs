using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly DbCloudzyContext _context;
        public ProductVariantRepository(DbCloudzyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ProductVariant productVariant)
        {
            await _context.ProductVariants.AddAsync(productVariant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant != null)
            {
                _context.ProductVariants.Remove(variant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync(int productId)
        {
            return await _context.ProductVariants
                .Include(pv => pv.Size)
                .Where(pv => pv.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductVariant> GetByIdAsync(int variantId)
        {
            return await _context.ProductVariants.FindAsync(variantId);
        }

        public async Task UpdateAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
        }
    }
}
