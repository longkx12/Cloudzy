using Cloudzy.Models.ViewModels.AdminBrand;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IBrandService
    {
        Task<IPagedList<BrandListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<BrandEditViewModel> GetByIdAsync(int id);
        Task AddAsync(BrandCreateViewModel model);
        Task UpdateAsync(BrandEditViewModel model);
        Task DeleteAsync(int id);
    }
}
