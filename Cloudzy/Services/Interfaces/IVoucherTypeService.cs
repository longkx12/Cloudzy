using Cloudzy.Models.ViewModels.AdminVoucherType;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IVoucherTypeService
    {
        Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<EditViewModel> GetByIdAsync(int id);
        Task AddAsync(CreateViewModel model);
        Task UpdateAsync(EditViewModel model);
        Task DeleteAsync(int id);
    }
}
