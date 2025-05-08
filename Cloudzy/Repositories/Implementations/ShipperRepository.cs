using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class ShipperRepository : IShipperRepository
    {
        private readonly DbCloudzyContext _context;

        public ShipperRepository(DbCloudzyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllShippersAsync()
        {
            return await _context.Users
                .Where(u => u.Role.RoleName == "Shipper")
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetProcessingOrdersAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == "Processing")
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetShippingOrdersByShipperIdAsync(int shipperId)
        {
            return await _context.Orders
                .Where(o => o.Status == "Shipping" && o.ShipperId == shipperId)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetDeliveredOrdersByShipperIdAsync(int shipperId)
        {
            return await _context.Orders
                .Where(o => o.Status == "Delivered" && o.ShipperId == shipperId)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.ProductImages)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Size)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> AcceptOrderAsync(int orderId, int shipperId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Processing")
            {
                return false;
            }

            order.Status = "Shipping";
            order.ShipperId = shipperId;
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeliverOrderAsync(int orderId, int shipperId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null || order.Status != "Shipping" || order.ShipperId != shipperId)
            {
                return false;
            }

            order.Status = "Delivered";
            order.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}