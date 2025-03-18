using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class ImportDetail
{
    public int ImportDetailId { get; set; }

    public int? ImportId { get; set; }

    public int? VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Import? Import { get; set; }

    public virtual ProductVariant? Variant { get; set; }
}
