using Cloudzy.Models.ViewModels.ProductReview;

namespace Cloudzy.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ProductReviewViewModel>> GetProductReviewsAsync(int productId);
        Task<bool> AddReviewAsync(int productId, int userId, int rating, string comment);
        Task<bool> DeleteReviewAsync(int reviewId, int userId);
    }
}
