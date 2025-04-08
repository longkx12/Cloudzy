using Cloudzy.Models.ViewModels.AdminProductVariant;
using X.PagedList;

namespace Cloudzy.Models.ViewModels.Product
{
    public class ProductViewModel
    {
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<BrandViewModel> Brands { get; set; }
        public IPagedList<ProductListViewModel> Products { get; set; }
        public FilterViewModel Filter { get; set; }
    }
}
