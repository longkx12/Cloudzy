using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminProductVariant;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _repository;
        private readonly DbCloudzyContext _context;
        public ProductVariantService(IProductVariantRepository repository,DbCloudzyContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task AddAsync(CreateViewModel model)
        {
            if (!await _context.Products.AnyAsync(p => p.ProductId == model.ProductId))
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }

            var productVariant = new ProductVariant
            {
                ProductId = model.ProductId,
                SizeId = model.SizeId,
                Stock = model.Stock
            };
            await _repository.AddAsync(productVariant);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IPagedList<ListViewModel>> GetAllAsync(int productId, int pageNumber, int pageSize)
        {
            var variants = await _repository.GetAllAsync(productId);
            return variants.Select((pv, index) => new ListViewModel
            {
                STT = index + 1,
                ProductId = pv.ProductId,
                VariantId = pv.VariantId,
                SizeName = pv.Size.SizeName,
                HeightRange = $"{pv.Size.HeightMin} - {pv.Size.HeightMax} cm",
                WeightRange = $"{pv.Size.WeightMin} - {pv.Size.WeightMax} kg",
                Stock = pv.Stock
            }).ToPagedList(pageNumber, pageSize);
        }

        public async Task<EditViewModel?> GetByIdAsync(int id)
        {
            var variant = await _repository.GetByIdAsync(id);
            if (variant == null) return null;

            return new EditViewModel
            {
                VariantId = variant.VariantId,
                ProductId = variant.ProductId ?? 0,
                SizeId = variant.SizeId ?? 0,
                Stock = variant.Stock
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            var variant = await _repository.GetByIdAsync(model.VariantId);
            if (variant != null)
            {
                variant.ProductId = model.ProductId;
                variant.SizeId = model.SizeId;
                variant.Stock = model.Stock;

                await _repository.UpdateAsync(variant);
            }
        }
    }
}
