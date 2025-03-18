using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Address { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public int? DiscountCodeId { get; set; }

    public virtual DiscountCode? DiscountCode { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}
