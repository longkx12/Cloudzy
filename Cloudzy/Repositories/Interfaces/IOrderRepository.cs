using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}
