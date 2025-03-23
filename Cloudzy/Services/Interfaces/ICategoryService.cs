using Cloudzy.Models.ViewModels.AdminCategory;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IPagedList<CategoryListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<CategoryViewModel> GetByIdAsync(int id);
        Task AddAsync(CategoryViewModel model);
        Task UpdateAsync(CategoryEditViewModel model);
        Task DeleteAsync(int id);
    }
}
