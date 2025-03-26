using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminDiscountCode;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly IDiscountCodeRepository _discountCodeRepository;
        private readonly DbCloudzyContext _context;
        public DiscountCodeService(IDiscountCodeRepository discountCodeRepository, DbCloudzyContext context)
        {
            _discountCodeRepository = discountCodeRepository;
            _context = context;
        }
        public async Task AddAsync(DiscountCodeCreateViewModel model)
        {
            var discountCode = new DiscountCode
            {
                Code = model.Code,
                VoucherTypeId = model.VoucherTypeId,
                Quantity = model.Quantity,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            await _discountCodeRepository.AddAsync(discountCode);
        }

        public async Task DeleteAsync(int id)
        {
            await _discountCodeRepository.DeleteAsync(id);
        }

        public async Task<IPagedList<DiscountCodeListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var discountCode = await _discountCodeRepository.GetAllAsync();
            var pageDiscountCode = discountCode.Select((d, index) => new DiscountCodeListViewModel
            {
                DiscountCodeId = d.DiscountCodeId,
                STT = index + 1,
                Code = d.Code,
                VoucherTypeName = d.VoucherType?.VoucherTypeName ?? "N/A",
                Quantity = d.Quantity,
                UsedQuantity = _context.Orders.Count(o => o.DiscountCodeId == d.DiscountCodeId),
                StartDate = d.StartDate,
                EndDate = d.EndDate
            }).ToPagedList(pageNumber,pageSize);
            return pageDiscountCode;
        }

        public async Task<DiscountCodeEditViewModel> GetByIdAsync(int id)
        {
            var discountCode = await _discountCodeRepository.GetByIdAsync(id);
            if (discountCode == null) return null;
            return new DiscountCodeEditViewModel
            {
                DiscountCodeId = discountCode.DiscountCodeId,
                
            };
        }

        public async Task UpdateAsync(DiscountCodeEditViewModel model)
        {
            var discountCode = await _discountCodeRepository.GetByIdAsync(model.DiscountCodeId);
            if (discountCode != null)
            {
                discountCode.Code = model.Code;
                discountCode.VoucherTypeId = model.VoucherTypeId;
                discountCode.Quantity = model.Quantity;
                discountCode.StartDate = model.StartDate;
                discountCode.EndDate = model.EndDate;

                await _discountCodeRepository.UpdateAsync(discountCode);
            }
        }
    }
}
