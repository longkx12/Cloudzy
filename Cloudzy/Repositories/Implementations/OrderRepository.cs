using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminOrder;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbCloudzyContext _context;

        public OrderRepository(DbCloudzyContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.DiscountCode)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
