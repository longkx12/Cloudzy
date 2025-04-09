using Cloudzy.Models.Domain;

namespace Cloudzy.Models.ViewModels.Product
{
    public class FilterViewModel
    {
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public string? SearchTerm { get; set; }
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Brand> Brands { get; set; } = new List<Brand>();
    }
}
