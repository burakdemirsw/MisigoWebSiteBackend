using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class ProductCountModel3
    {
        public int? Status { get; set; }
        public string? Description { get; set; }
        public string? ShelfNo { get; set; }
        public string? Barcode { get; set; }
        public string? BatchCode { get; set; }
        public int Quantity { get; set; }

    }
}
