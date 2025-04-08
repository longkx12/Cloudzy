using Cloudzy.Models.ViewModels.AdminProduct;
using Cloudzy.Models.ViewModels.Product;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IProductService
    {
        Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<IPagedList<ProductListViewModel>> GetFilteredProductsAsync(int pageNumber, int pageSize, int? categoryId, int? brandId, string? searchTerm);
        Task<EditViewModel> GetByIdAsync(int id);
        Task AddAsync(CreateViewModel model);
        Task UpdateAsync(EditViewModel model);
        Task DeleteAsync(int id);
    }
}
