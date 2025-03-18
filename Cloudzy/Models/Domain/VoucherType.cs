using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class VoucherType
{
    public int VoucherTypeId { get; set; }

    public string VoucherTypeName { get; set; } = null!;

    public decimal Value { get; set; }

    public decimal MinimumValue { get; set; }

    public decimal? MaximumValue { get; set; }

    public virtual ICollection<DiscountCode> DiscountCodes { get; set; } = new List<DiscountCode>();
}
