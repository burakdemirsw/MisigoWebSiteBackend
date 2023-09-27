﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class CreatePurchaseInvoice
    {
        public string? ShelfNo { get; set; }
        public string? Office { get; set; }
        public string? Warehouse { get; set; }
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? CurrAccCode { get; set; }
        public string? OrderNumber { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? Currency { get; set; }

    }
}