using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IProductVariantRepository
    {
        Task<IEnumerable<ProductVariant>> GetAllAsync(int productId);
        Task<ProductVariant> GetByIdAsync(int variantId);
        Task AddAsync(ProductVariant productVariant);
        Task UpdateAsync(ProductVariant productVariant);
        Task DeleteAsync(int id);
    }
}
