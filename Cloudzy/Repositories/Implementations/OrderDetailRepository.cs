using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly DbCloudzyContext _context;

        public OrderDetailRepository(DbCloudzyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Variant)
                .Include(od => od.Variant.Product)
                .Include(od => od.Variant.Product.ProductImages)
                .Include(od => od.Variant.Size)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }
    }
}
