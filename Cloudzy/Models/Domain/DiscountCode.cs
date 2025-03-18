using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class DiscountCode
{
    public int DiscountCodeId { get; set; }

    public string Code { get; set; } = null!;

    public int? VoucherTypeId { get; set; }

    public int Quantity { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual VoucherType? VoucherType { get; set; }
}
