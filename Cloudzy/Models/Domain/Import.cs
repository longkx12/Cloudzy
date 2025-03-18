using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class Import
{
    public int ImportId { get; set; }

    public int? SupplierId { get; set; }

    public DateTime ImportDate { get; set; }

    public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();

    public virtual Supplier? Supplier { get; set; }
}
