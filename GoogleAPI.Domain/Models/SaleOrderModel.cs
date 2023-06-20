using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class SaleOrderModel
    {
        public DateTime OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public string CurrAccCode { get; set; }
        public string CurrAccDescription { get; set; }
        public string SalespersonCode { get; set; }
        public Double Qty1 { get; set; }
        public decimal Tutar { get; set; }
    }
}
