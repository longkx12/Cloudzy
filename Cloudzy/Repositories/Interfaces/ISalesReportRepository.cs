using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface ISalesReportRepository
    {
        Task<List<Order>> GetDeliveredOrdersAsync(DateTime? startDate, DateTime? endDate, int? categoryId);
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Dictionary<int, string>> GetCategoryNamesAsync(IEnumerable<int> categoryIds);
        Task<Dictionary<int, string>> GetProductCategoriesAsync(IEnumerable<int> productIds);
        Task<Dictionary<int, string>> GetCustomerNamesAsync(IEnumerable<int> userIds);
        Task<Dictionary<int, List<OrderDetail>>> GetOrderDetailsAsync(IEnumerable<int> orderIds);
        Task<Dictionary<int, Product>> GetProductsAsync(IEnumerable<int> productIds);
    }
}
