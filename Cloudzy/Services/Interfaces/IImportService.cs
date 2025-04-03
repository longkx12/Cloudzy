using Cloudzy.Models.ViewModels.AdminImport;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IImportService
    {
        Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<EditViewModel> GetByIdAsync(int id);
        Task AddAsync(CreateViewModel model);
        Task UpdateAsync(EditViewModel model);
        Task DeleteAsync(int id);
    }
}
