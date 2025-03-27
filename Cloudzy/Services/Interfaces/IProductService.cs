using Cloudzy.Models.ViewModels.AdminProduct;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IProductService
    {
        Task<IPagedList<ProductListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<ProductEditViewModel> GetByIdAsync(int id);
        Task AddAsync(ProductCreateViewModel model);
        Task UpdateAsync(ProductEditViewModel model);
        Task DeleteAsync(int id);
    }
}
