using Cloudzy.Models.ViewModels.AdminVoucherType;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IVoucherTypeService
    {
        Task<IPagedList<VoucherTypeListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<VoucherTypeEditViewModel> GetByIdAsync(int id);
        Task AddAsync(VoucherTypeCreateViewModel model);
        Task UpdateAsync(VoucherTypeEditViewModel model);
        Task DeleteAsync(int id);
    }
}
