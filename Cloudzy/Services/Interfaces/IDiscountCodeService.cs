using Cloudzy.Models.ViewModels.AdminDiscountCode;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IDiscountCodeService
    {
        Task<IPagedList<DiscountCodeListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<DiscountCodeEditViewModel> GetByIdAsync(int id);
        Task AddAsync(DiscountCodeCreateViewModel model);
        Task UpdateAsync(DiscountCodeEditViewModel model);
        Task DeleteAsync(int id);
    }
}