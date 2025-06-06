﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cloudzy.Models.ViewModels.AdminProductVariant
{
    public class EditViewModel
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Stock { get; set; }
        public IEnumerable<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
    }
}
