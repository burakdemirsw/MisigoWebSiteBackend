using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class SaleOrderModel
    {
        public DateTime OrderDate { get; set; }
        public string? OrderNumber { get; set; }
        public string? CurrAccCode { get; set; }
        public string? CurrAccDescription { get; set; }
        public string? SalespersonCode { get; set; }
        public double? Qty1 { get; set; }
        public int? Price { get; set; }
    }
}
