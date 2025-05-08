using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IShipperRepository
    {
        Task<IEnumerable<User>> GetAllShippersAsync();
        Task<IEnumerable<Order>> GetProcessingOrdersAsync();
        Task<IEnumerable<Order>> GetShippingOrdersByShipperIdAsync(int shipperId);
        Task<IEnumerable<Order>> GetDeliveredOrdersByShipperIdAsync(int shipperId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<bool> AcceptOrderAsync(int orderId, int shipperId);
        Task<bool> DeliverOrderAsync(int orderId, int shipperId);
    }
}