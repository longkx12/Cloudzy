using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment) || rating < 1 || rating > 5)
            {
                TempData["ToastMessage"] = "Vui lòng nhập đánh giá và chọn số sao!";
                TempData["ToastType"] = "error";
                return RedirectToAction("Index", "ProductDetail", new { productId });
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            var result = await _reviewService.AddReviewAsync(productId, userId, rating, comment);

            if (result)
            {
                TempData["ToastMessage"] = "Đánh giá của bạn đã được gửi thành công!";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "Có lỗi xảy ra. Vui lòng thử lại sau!";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction("Index", "ProductDetail", new { productId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int reviewId, int productId)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            var result = await _reviewService.DeleteReviewAsync(reviewId, userId);

            if (result)
            {
                TempData["ToastMessage"] = "Đã xóa đánh giá thành công!";
                TempData["ToastType"] = "success";
            }
            else
            {
                TempData["ToastMessage"] = "Không thể xóa đánh giá này!";
                TempData["ToastType"] = "error";
            }

            return RedirectToAction("Index", "ProductDetail", new { productId });
        }
    }
}
