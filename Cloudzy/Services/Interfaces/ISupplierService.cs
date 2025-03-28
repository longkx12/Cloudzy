using Cloudzy.Models.ViewModels.AdminSupplier;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<EditViewModel> GetByIdAsync(int id);
        Task AddAsync(CreateViewModel model);
        Task UpdateAsync(EditViewModel model);
        Task DeleteAsync(int id);
    }
}
