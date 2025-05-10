using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetReturnRequestsAsync();
        Task<Order> GetByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAndShipperAsync(int orderId, string status, int? shipperId);
    }
}