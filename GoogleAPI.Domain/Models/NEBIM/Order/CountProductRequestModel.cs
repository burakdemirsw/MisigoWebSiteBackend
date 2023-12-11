using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class CountProductRequestModel
    {
        public string? Barcode { get; set; }
        public string? ShelfNo { get; set; }

        public int? Qty { get; set; }
        public string? OrderNumber { get; set; }


    }

    public class CountProductRequestModel2
    {
        public string? Barcode { get; set; }
        public string? ShelfNo { get; set; }
        public string? BatchCode { get; set; }
        public string? Office { get; set; }
        public string? WarehouseCode { get; set; }
        public string? OrderNo { get; set; }
        public int? Quantity { get; set; }
        public string? CurrAccCode { get; set; }
        public bool? IsReturn { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? TaxTypeCode { get; set; }

    }
}
