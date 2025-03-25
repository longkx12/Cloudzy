using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminVoucherType;
using Cloudzy.Repositories.Implementations;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class VoucherTypeService : IVoucherTypeService
    {
        private readonly IVoucherTypeRepository _voucherTypeRepository;
        public VoucherTypeService(IVoucherTypeRepository voucherTypeRepository)
        {
            _voucherTypeRepository = voucherTypeRepository;
        }
        public async Task AddAsync(VoucherTypeCreateViewModel model)
        {
            var voucherType = new VoucherType
            {
                VoucherTypeName = model.VoucherTypeName,
                Value=model.Value,
                MinimumValue = model.MinimumValue,
                MaximumValue = model.MaximumValue
            };
            await _voucherTypeRepository.AddAsync(voucherType);
        }

        public async Task DeleteAsync(int id)
        {
            await _voucherTypeRepository.DeleteAsync(id);
        }

        public async Task<IPagedList<VoucherTypeListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var voucherTypes = await _voucherTypeRepository.GetAllAsync();
            var pageVoucherTypes = voucherTypes.Select((v, index) => new VoucherTypeListViewModel
            {
                VoucherTypeId = v.VoucherTypeId,
                STT = index + 1,
                VoucherTypeName = v.VoucherTypeName,
                Value = v.Value,
                MinimumValue = v.MinimumValue,
                MaximumValue = v.MaximumValue
            }).ToPagedList(pageNumber,pageSize);
            return pageVoucherTypes;
        }

        public async Task<VoucherTypeEditViewModel> GetByIdAsync(int id)
        {
            var voucherType = await _voucherTypeRepository.GetByIdAsync(id);
            if (voucherType == null) return null;

            return new VoucherTypeEditViewModel
            {
                VoucherTypeId = voucherType.VoucherTypeId,
                VoucherTypeName = voucherType.VoucherTypeName,
                Value=voucherType.Value,
                MinimumValue=voucherType.MinimumValue,
                MaximumValue=voucherType.MaximumValue
            };
        }

        public async Task UpdateAsync(VoucherTypeEditViewModel model)
        {
            var voucherType = await _voucherTypeRepository.GetByIdAsync(model.VoucherTypeId);
            if (voucherType == null) return;

            voucherType.VoucherTypeName = model.VoucherTypeName;
            voucherType.Value = model.Value;
            voucherType.MinimumValue = model.MinimumValue;
            voucherType.MaximumValue = model.MaximumValue;

            await _voucherTypeRepository.UpdateAsync(voucherType);
        }
    }
}
