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
        public async Task AddAsync(CreateViewModel model)
        {
            //Kiểm tra ngày bắt đầu và ngày kết thúc
            if (model.EndDate <= model.StartDate)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu");
            }

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

        public async Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var discountCode = await _discountCodeRepository.GetAllAsync();
            var pageDiscountCode = discountCode.Select((d, index) => new ListViewModel
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

        public async Task<EditViewModel> GetByIdAsync(int id)
        {
            var discountCode = await _discountCodeRepository.GetByIdAsync(id);
            if (discountCode == null) return null;
            return new EditViewModel
            {
                DiscountCodeId = discountCode.DiscountCodeId,
                Code = discountCode.Code,
                VoucherTypeId = discountCode.VoucherTypeId,
                Quantity = discountCode.Quantity,
                StartDate = discountCode.StartDate,
                EndDate = discountCode.EndDate
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            //Kiểm tra ngày bắt đầu và ngày kết thúc
            if (model.EndDate <= model.StartDate)
            {
                throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu");
            }

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
