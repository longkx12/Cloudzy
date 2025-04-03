using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminImport;
using Cloudzy.Repositories.Implementations;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class ImportService : IImportService
    {
        private readonly IImportRepository _repository;
        public ImportService(IImportRepository repository)
        {
            _repository = repository;
        }
        public async Task AddAsync(CreateViewModel model)
        {
            var import = new Import
            {
                SupplierId = model.SupplierId,
                ImportDate = model.ImportDate
            };
            await _repository.AddAsync(import);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var import = await _repository.GetAllAsync();
            var pageImport = import.Select((i, index) => new ListViewModel
            {
                ImportId = i.ImportId,
                STT = index + 1,
                SupplierName = i.Supplier.SupplierName,
                PhoneNumber = i.Supplier.PhoneNumber,
                ImportDate = i.ImportDate
            }).ToPagedList(pageNumber, pageSize);
            return pageImport;
        }

        public async Task<EditViewModel> GetByIdAsync(int id)
        {
            var import = await _repository.GetByIdAsync(id);
            if (import == null) return null;
            return new EditViewModel
            {
                ImportId = import.ImportId,
                SupplierId = import.SupplierId,
                ImportDate = import.ImportDate
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            var import = await _repository.GetByIdAsync(model.ImportId);
            if (import != null)
            {
                import.SupplierId = model.SupplierId;
                import.ImportDate = model.ImportDate;

                await _repository.UpdateAsync(import);
            }
        }
    }
}
