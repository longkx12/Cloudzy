using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminImportDetail;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class ImportDetailService : IImportDetailService
    {
        private readonly IImportDetailRepository _repository;
        private readonly DbCloudzyContext _context;
        public ImportDetailService(IImportDetailRepository repository,DbCloudzyContext context)
        {
            _repository = repository;
            _context = context;
        }
        public async Task AddAsync(CreateViewModel model)
        {
            if (!await _context.Imports.AnyAsync(i => i.ImportId == model.ImportId))
            {
                throw new Exception("Không tìm thấy nhập hàng");
            }
            var detail = new ImportDetail
            {
                ImportId = model.ImportId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Price = model.Price
            };
            await _repository.AddAsync(detail);
            
        }
        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IPagedList<ListViewModel>> GetAllAsync(int importId, int pageNumber, int pageSize)
        {
            var details = await _repository.GetAllAsync(importId);
            return details.Select((d, index) => new ListViewModel
            {
                STT = index + 1,
                ImportId = d.ImportId,
                ImportDetailId = d.ImportDetailId,
                ProductName = d.Product.ProductName,
                Quantity = d.Quantity,
                Price = d.Price
            }).ToPagedList(pageNumber, pageSize);
        }

        public async Task<EditViewModel> GetByIdAsync(int id)
        {
            var detail = await _repository.GetByIdAsync(id);
            if (detail == null) return null;

            return new EditViewModel
            {
                ImportDetailId = detail.ImportDetailId,
                ImportId = detail.ImportId ?? 0,
                ProductId = detail.ProductId ?? 0,
                Quantity = detail.Quantity,
                Price = detail.Price
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            var detail = await _repository.GetByIdAsync(model.ImportDetailId);
            if (detail != null)
            {
                detail.ImportId = model.ImportId;
                detail.ProductId = model.ProductId;
                detail.Quantity = model.Quantity;
                detail.Price = model.Price;

                await _repository.UpdateAsync(detail);
            }
        }
    }
}
