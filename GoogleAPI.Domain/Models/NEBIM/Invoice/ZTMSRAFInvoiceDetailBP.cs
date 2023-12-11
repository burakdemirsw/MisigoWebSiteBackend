using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class ZTMSRAFInvoiceDetailBP
    {
        [Key]
        public Guid ID { get; set; }

        public string? UsedBarcode { get; set; }
        public string? ItemCode { get; set; }

        public string? BatchCode { get; set; }

        [Required]
        public int LDisRate1 { get; set; }

        public string? VatRate { get; set; }

        public decimal? Price { get; set; }

        public decimal? Amount { get; set; }

        public int? Qty1 { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string? ITAttributes { get; set; }

        [Required]
        public string? OrderNumber { get; set; }

        [Required]
        public string? OrdernumberRAF { get; set; }
    }
}
