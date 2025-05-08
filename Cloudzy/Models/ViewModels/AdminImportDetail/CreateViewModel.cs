using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace Cloudzy.Models.ViewModels.AdminImportDetail
{
    public class CreateViewModel
    {
        public int ImportId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn size.")]
        public int SizeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải > 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        public decimal Price { get; set; }
        public IEnumerable<SelectListItem> Product { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
    }
}