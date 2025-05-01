using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.ProductReview;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;

namespace Cloudzy.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
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
    }
}
