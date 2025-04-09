using Cloudzy.Models.ViewModels.Cart;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<IPagedList<CartItemListViewModel>> GetAllAsync(int cartId, int pageNumber, int pageSize);
        Task<CartItemEditViewModel> GetByIdAsync(int id);
        Task AddAsync(CartItemCreateViewModel model);
        Task UpdateAsync(CartItemEditViewModel model);
        Task DeleteAsync(int id);
    }
}
