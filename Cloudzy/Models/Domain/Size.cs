using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class Size
{
    public int SizeId { get; set; }

    public string SizeName { get; set; } = null!;

    public int? Height { get; set; }

    public int? Weight { get; set; }

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
