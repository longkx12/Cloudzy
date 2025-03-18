using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Material { get; set; }

    public string? ProductDescription { get; set; }

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    public int? SupplierId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Supplier? Supplier { get; set; }
}
