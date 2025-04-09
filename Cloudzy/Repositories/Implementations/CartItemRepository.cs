using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(DbCloudzyContext dbCloudzyContext) : base(dbCloudzyContext)
        {
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync(int cartId)
        {
            return await _dbSet
                .Include(c => c.Variant)
                    .ThenInclude(c => c.Product)
                .Include(c => c.Variant)
                    .ThenInclude(c => c.Size)
                .Where(c => c.CartId == cartId)
                .ToListAsync();
        }
    }
}
