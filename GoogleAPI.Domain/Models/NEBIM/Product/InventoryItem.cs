using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class InventoryItemModel
    {
        public string? PhotoUrl { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemDescription { get; set; }
        public string? ShelfNo { get; set; }
        public int Quantity { get; set; }
        public int TransferQty { get; set; }
        public string Barcode { get; set; }


    }
}
