using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? CartId { get; set; }

    public int? VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual ShoppingCart? Cart { get; set; }

    public virtual ProductVariant? Variant { get; set; }
}
