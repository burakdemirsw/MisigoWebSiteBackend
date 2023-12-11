using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM
{
    public class QrCode
    {
        public Guid Id { get; set; }
        public string? Barcode { get; set; }
        public string? ShelfNo { get; set; }
        public string? BatchCode { get; set; }
        public int   Quantity { get; set; }

        public DateTime? CreatedDate { get; set; }


    }
}
