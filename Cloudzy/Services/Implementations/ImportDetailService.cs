using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminImportDetail;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class ImportDetailService : IImportDetailService
    {
        private readonly IImportDetailRepository _repository;
        private readonly DbCloudzyContext _context;

        public ImportDetailService(IImportDetailRepository repository, DbCloudzyContext context)
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
                SizeId = model.SizeId,
                Quantity = model.Quantity,
                Price = model.Price
            };
            await _repository.AddAsync(detail);

            var productVariant = await _context.ProductVariants
                .FirstOrDefaultAsync(pv => pv.ProductId == model.ProductId && pv.SizeId == model.SizeId);

            if (productVariant != null)
            {
                productVariant.Stock += model.Quantity;
            }
            else
            {
                _context.ProductVariants.Add(new ProductVariant
                {
                    ProductId = model.ProductId,
                    SizeId = model.SizeId,
                    Stock = model.Quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var detail = await _repository.GetByIdAsync(id);
            if (detail != null)
            {
                var productVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.ProductId == detail.ProductId && pv.SizeId == detail.SizeId);

                if (productVariant != null)
                {
                    productVariant.Stock -= detail.Quantity;
                    if (productVariant.Stock < 0)
                    {
                        productVariant.Stock = 0;
                    }
                    await _context.SaveChangesAsync();
                }
            }

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
                SizeName = d.Size?.SizeName ?? "N/A",
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
                SizeId = detail.SizeId ?? 0,
                Quantity = detail.Quantity,
                Price = detail.Price
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            var detail = await _repository.GetByIdAsync(model.ImportDetailId);
            if (detail != null)
            {
                var oldProductVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.ProductId == detail.ProductId && pv.SizeId == detail.SizeId);

                if (oldProductVariant != null)
                {
                    oldProductVariant.Stock -= detail.Quantity;
                    if (oldProductVariant.Stock < 0)
                    {
                        oldProductVariant.Stock = 0;
                    }
                }

                detail.ImportId = model.ImportId;
                detail.ProductId = model.ProductId;
                detail.SizeId = model.SizeId;
                detail.Quantity = model.Quantity;
                detail.Price = model.Price;

                await _repository.UpdateAsync(detail);

                var newProductVariant = await _context.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.ProductId == model.ProductId && pv.SizeId == model.SizeId);

                if (newProductVariant != null)
                {
                    newProductVariant.Stock += model.Quantity;
                }
                else
                {
                    _context.ProductVariants.Add(new ProductVariant
                    {
                        ProductId = model.ProductId,
                        SizeId = model.SizeId,
                        Stock = model.Quantity
                    });
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SelectListItem>> GetSizesByProductIdAsync(int productId)
        {
            var sizes = await _context.ProductVariants
                .Where(pv => pv.ProductId == productId)
                .Select(pv => pv.Size)
                .Distinct()
                .ToListAsync();

            if (!sizes.Any())
            {
                sizes = await _context.Sizes.ToListAsync();
            }

            return sizes
                .Where(s => s != null)
                .Select(s => new SelectListItem
                {
                    Value = s.SizeId.ToString(),
                    Text = s.SizeName
                })
                .ToList();
        }
    }
}