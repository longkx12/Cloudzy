using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(DbCloudzyContext dbCloudzyContext) : base(dbCloudzyContext)
        {
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync(int productId)
        {
            return await _dbSet
                .Include(pv => pv.Size)
                .Include(pv => pv.Product)
                    .ThenInclude(pv => pv.ProductImages)
                .Include(pv => pv.Product.Category)
                .Include(pv => pv.Product.Brand)
                .Where(pv => pv.ProductId == productId)
                .ToListAsync();
        }
    }
}
