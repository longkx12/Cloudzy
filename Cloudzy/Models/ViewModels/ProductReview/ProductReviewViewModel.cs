namespace Cloudzy.Models.ViewModels.ProductReview
{
    public class ProductReviewViewModel
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserImg { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
