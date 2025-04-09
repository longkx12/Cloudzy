using Cloudzy.Models.ViewModels.Product;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrandRepository _brandRepository;

        public ProductController(IProductService productService, IBrandService brandService, ICategoryService categoryService, ICategoryRepository categoryRepository, IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _brandService = brandService;
            _categoryService = categoryService;
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new FilterViewModel
            {
                Categories = await _categoryRepository.GetAllAsync(),
                Brands = await _brandRepository.GetAllAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Load(int page = 1, int? categoryId = null, int? brandId = null, string? searchTerm = null)
        {
            // Trường hợp nếu chọn Tất cả
            if (categoryId.HasValue && categoryId.Value == 0)
            {
                categoryId = null;
            }

            if (brandId.HasValue && brandId.Value == 0)
            {
                brandId = null;
            }

            int pageSize = 6;
            var products = await _productService.GetFilteredProductsAsync(page, pageSize, categoryId, brandId, searchTerm);

            return PartialView("_ProductListPartial", products);
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
