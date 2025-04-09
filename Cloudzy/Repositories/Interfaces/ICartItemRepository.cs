using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetAllAsync(int cartId);
    }
}
