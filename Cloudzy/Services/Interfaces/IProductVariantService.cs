using Cloudzy.Models.ViewModels.AdminProductVariant;
using Cloudzy.Models.ViewModels.ProductDetail;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IProductVariantService
    {
        Task<IPagedList<ListViewModel>> GetAllAsync(int productId, int pageNumber, int pageSize);
        Task<IList<ProductDetailViewModel>> GetDetailAsync(int productId);
        Task<EditViewModel?> GetByIdAsync(int id);
        Task AddAsync(CreateViewModel model);
        Task UpdateAsync(EditViewModel model);
        Task DeleteAsync(int id);
    }
}
