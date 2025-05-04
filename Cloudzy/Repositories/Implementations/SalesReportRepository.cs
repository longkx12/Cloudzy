using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Cloudzy.Repositories.Implementations
{
    public class SalesReportRepository : ISalesReportRepository
    {
        private readonly DbCloudzyContext _context;
        public SalesReportRepository(DbCloudzyContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetDeliveredOrdersAsync(DateTime? startDate, DateTime? endDate, int? categoryId)
        {
            var query = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.Category)
                .Include(o => o.User)
                .Where(o => o.Status == "Delivered");
            if (startDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= endDate.Value.AddDays(1).AddSeconds(-1));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(o => o.OrderDetails.Any(od => od.Variant.Product.CategoryId == categoryId.Value));
            }
            return await query.ToListAsync();
        }
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
        public async Task<Dictionary<int, string>> GetCategoryNamesAsync(IEnumerable<int> categoryIds)
        {
            return await _context.Categories
                .Where(c => categoryIds.Contains(c.CategoryId))
                .ToDictionaryAsync(c => c.CategoryId, c => c.CategoryName);
        }
        public async Task<Dictionary<int, string>> GetProductCategoriesAsync(IEnumerable<int> productIds)
        {
            var productCategories = await _context.Products
                .Where(p => productIds.Contains(p.ProductId))
                .Select(p => new
                {
                    p.ProductId,
                    CategoryName = p.Category.CategoryName
                })
                .ToDictionaryAsync(p => p.ProductId, p => p.CategoryName);
            return productCategories;
        }
        public async Task<Dictionary<int, string>> GetCustomerNamesAsync(IEnumerable<int> userIds)
        {
            return await _context.Users
                .Where(u => userIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId, u => u.Fullname);
        }
        public async Task<Dictionary<int, List<OrderDetail>>> GetOrderDetailsAsync(IEnumerable<int> orderIds)
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Variant)
                    .ThenInclude(v => v.Product)
                .Where(od => orderIds.Contains(od.OrderId.Value))
                .ToListAsync();
            return orderDetails.GroupBy(od => od.OrderId.Value)
                .ToDictionary(group => group.Key, group => group.ToList());
        }
        public async Task<Dictionary<int, Product>> GetProductsAsync(IEnumerable<int> productIds)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId, p => p);
        }
    }
}