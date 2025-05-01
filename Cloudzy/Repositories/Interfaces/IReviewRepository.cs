using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetProductReviewsAsync(int productId);
        Task<bool> AddReviewAsync(Review review);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<Review> GetReviewByIdAsync(int reviewId);
    }
}
