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

        public async Task<Order> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Shipper)
                .Include(o => o.DiscountCode)
                .Include(o => o.DiscountCode.VoucherType)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> UpdateOrderStatusAndShipperAsync(int orderId, string status, int? shipperId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            order.UpdatedAt = DateTime.Now;

            if (shipperId.HasValue)
            {
                order.ShipperId = shipperId;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}