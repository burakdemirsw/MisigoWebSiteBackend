using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class OrderBillingRequestModel
    {
        public string? OrderNo { get; set; }
        public bool InvoiceType { get; set; }
        public int? InvoiceModel { get; set;}
        public string? SalesPersonCode { get; set; }
        public string? Currency { get; set; }
    }
}
