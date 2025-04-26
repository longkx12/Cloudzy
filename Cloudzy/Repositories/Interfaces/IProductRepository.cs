using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminProduct;
using Cloudzy.Models.ViewModels.Product;
using X.PagedList;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetFilteredProductsAsync(int? categoryId, int? brandId, string? searchTerm);
        Task<Product> GetProductByVariantIdAsync(int variantId);
    }
}
