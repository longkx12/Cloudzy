using Cloudzy.Models.ViewModels.AdminImportDetail;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IImportDetailService
    {
        Task<IPagedList<ListViewModel>> GetAllAsync(int importId, int pageNumber, int pageSize);
        Task<EditViewModel> GetByIdAsync(int id);
        Task AddAsync(CreateViewModel model);
        Task UpdateAsync(EditViewModel model);
        Task DeleteAsync(int id);
    }
}
