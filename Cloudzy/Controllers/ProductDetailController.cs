using Cloudzy.Data;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly IProductVariantService _productVariantService;
        private readonly DbCloudzyContext _context;
        public ProductDetailController(IProductVariantService productVariantService, DbCloudzyContext context)
        {
            _productVariantService = productVariantService;
            _context = context;
        }
        public async Task<IActionResult> Index(int productId)
        {
            ViewBag.ProductId = productId;
            var productVariants = await _productVariantService.GetDetailAsync(productId);
            return View(productVariants);
        }
    }
}
