using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly DbCloudzyContext _context;
        public ShoppingCartRepository(DbCloudzyContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShoppingCart entity)
        {
            await _context.ShoppingCarts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cart = await _context.ShoppingCarts.FindAsync(id);
            if (cart != null)
            {
                _context.ShoppingCarts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync()
        {
            return await _context.ShoppingCarts
                .Include(c => c.User)
                .ToListAsync();
        }

        public async Task<ShoppingCart> GetByIdAsync(int id)
        {
            return await _context.ShoppingCarts.FindAsync(id);
        }

        public async Task UpdateAsync(ShoppingCart entity)
        {
            _context.ShoppingCarts.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
