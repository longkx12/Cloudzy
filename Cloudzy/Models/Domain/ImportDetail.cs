﻿using System;
using System.Collections.Generic;

namespace Cloudzy.Models.Domain;

public partial class ImportDetail
{
    public int ImportDetailId { get; set; }

    public int? ImportId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public int? ProductId { get; set; }

    public int? SizeId { get; set; }

    public virtual Import? Import { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Size? Size { get; set; }
}
