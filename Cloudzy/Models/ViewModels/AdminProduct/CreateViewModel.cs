﻿using Cloudzy.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Cloudzy.Models.ViewModels.AdminProduct
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhãn hàng")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Chất liệu không được để trống")]
        [StringLength(50, ErrorMessage = "Chất liệu không được vượt quá 50 ký tự")]
        public string? Material { get; set; }

        [Required(ErrorMessage = "Giá gốc không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn 0")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Giá khuyến mãi không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá khuyến mãi phải lớn hơn 0")]
        [DiscountLessThanPrice("Price", ErrorMessage = "Giá khuyến mãi phải nhỏ hơn giá gốc")]
        public decimal? DiscountPrice { get; set; }

        public string? ProductDescription { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hình ảnh")]
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg", ".gif" })]
        public IFormFile Images { get; set; }
        public IEnumerable<SelectListItem> Category { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Brand { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Supplier { get; set; } = new List<SelectListItem>();
    }
}
