using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class Size
{
    public int SizeId { get; set; }

    public string SizeName { get; set; } = null!;

    public int HeightMin { get; set; }

    public int HeightMax { get; set; }

    public decimal WeightMin { get; set; }

    public decimal WeightMax { get; set; }

    public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}
