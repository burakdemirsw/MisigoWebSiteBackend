using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class ZTMSRAFSAYIM3
    {
        public Guid ID { get; set; }
        public string? ShelfNo { get; set; }
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? BatchCode { get; set; }
        public DateTime ItemDate { get; set; }
        public string? OrderNumber { get; set; }
        public decimal? Price { get; set; }
        public string? WareHouseCode { get; set; }
        public string? CurrAccCode { get; set; }
        public string? Itemcode { get; set; }
        public bool? IsReturn { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? TaxTypeCode { get; set; }
    }
}
