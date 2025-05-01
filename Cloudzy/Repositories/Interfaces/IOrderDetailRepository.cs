using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    }
}
