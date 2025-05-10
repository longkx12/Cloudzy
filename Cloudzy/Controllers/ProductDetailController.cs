using Cloudzy.Data;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly IProductVariantService _productVariantService;
        private readonly IReviewService _reviewService;
        private readonly DbCloudzyContext _context;

        public ProductDetailController(
            IProductVariantService productVariantService,
            IReviewService reviewService,
            DbCloudzyContext context)
        {
            _productVariantService = productVariantService;
            _reviewService = reviewService;
            _context = context;
        }

        public async Task<IActionResult> Index(int productId)
        {
            ViewBag.ProductId = productId;
            var productVariants = await _productVariantService.GetDetailAsync(productId);
            ViewBag.ProductReviews = await _reviewService.GetProductReviewsAsync(productId);

            return View(productVariants);
        }
    }
}