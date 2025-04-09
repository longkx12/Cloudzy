using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.Cart;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<IPagedList<ShoppingCartListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<ShoppingCart> GetByIdAsync(int id);
        Task AddAsync(ShoppingCartCreateViewModel model);
        Task DeleteAsync(int id);
    }
}
