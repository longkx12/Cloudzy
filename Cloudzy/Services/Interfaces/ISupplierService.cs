using Cloudzy.Models.ViewModels.AdminSupplier;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IPagedList<SupplierListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<SupplierEditViewModel> GetByIdAsync(int id);
        Task AddAsync(SupplierCreateViewModel model);
        Task UpdateAsync(SupplierEditViewModel model);
        Task DeleteAsync(int id);
    }
}
