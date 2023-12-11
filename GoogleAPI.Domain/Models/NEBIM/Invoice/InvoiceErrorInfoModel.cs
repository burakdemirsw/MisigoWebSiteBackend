using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class InvoiceErrorInfoModel
    {
        public string? CurrAccCode { get; set; }
        public string? ItemCode { get; set; }

        public string? BatchCode { get; set; }
        public string? State { get; set; }

    }
}
