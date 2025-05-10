using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.ProductReview;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly DbCloudzyContext _context;

        public ReviewService(
            IReviewRepository reviewRepository,
            DbCloudzyContext context)
        {
            _reviewRepository = reviewRepository;
            _context = context;
        }

        public async Task<List<ProductReviewViewModel>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _reviewRepository.GetProductReviewsAsync(productId);
            return reviews.Select(r => new ProductReviewViewModel
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId ?? 0,
                UserName = r.User?.Fullname ?? "Unknown",
                UserImg = r.User?.UserImg,
                ProductId = r.ProductId ?? 0,
                Rating = r.Rating,
                Comment = r.Comment ?? "",
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<bool> AddReviewAsync(int productId, int userId, int rating, string comment)
        {
            if (rating < 1 || rating > 5)
                return false;

            bool hasPurchased = await HasUserPurchasedProduct(userId, productId);
            if (!hasPurchased)
                return false;

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.Now
            };

            return await _reviewRepository.AddReviewAsync(review);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
                return false;

            return await _reviewRepository.DeleteReviewAsync(reviewId);
        }

        private async Task<bool> HasUserPurchasedProduct(int userId, int productId)
        {
            var variantIds = await _context.ProductVariants
                .Where(pv => pv.ProductId == productId)
                .Select(pv => pv.VariantId)
                .ToListAsync();

            if (!variantIds.Any())
                return false;

            return await _context.Orders
                .Where(o => o.UserId == userId && o.Status == "Delivered")
                .Join(_context.OrderDetails,
                    order => order.OrderId,
                    detail => detail.OrderId,
                    (order, detail) => new { Order = order, Detail = detail })
                .AnyAsync(x => variantIds.Contains(x.Detail.VariantId ?? 0));
        }

        public async Task<bool> CanUserReviewProduct(int userId, int productId)
        {
            return await HasUserPurchasedProduct(userId, productId);
        }
    }
}