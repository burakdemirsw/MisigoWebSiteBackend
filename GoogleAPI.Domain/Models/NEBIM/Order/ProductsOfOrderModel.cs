﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class ProductOfOrderModel
    {
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? ItemCode { get; set; }
        public string? ColorCode { get; set; }
        public string? ColorDescription { get; set; }
        public string? ItemDim1Code { get; set; }
        public string? RowNumber { get; set; }
        public string? ShelfNo { get; set; }
        public Guid PackageNo { get; set; }

    }

  
}
