using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class TransferModel
    {
        public string? ShelfNo { get; set; }
        public string? Barcode { get; set; }
        public int Quantity { get; set; }
        public int AvailableQty { get; set; }

        public string? BatchCode { get; set; }
        public string? Warehouse { get; set; }
        public string? WarehouseTo { get; set; }
        public string? PhotoUrl { get; set; }
        public string? ItemCode { get; set; }


    }
}
