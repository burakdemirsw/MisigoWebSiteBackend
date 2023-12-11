using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class ProductList_VM
    {
        public string? Description { get; set; }
        public string? WarehouseCode { get; set; }
        public string? PhotoUrl { get; set; }
        public string? ShelfNo{ get; set; }
        public string? ItemCode { get; set; }
        public string? BatchCode { get; set; }
        public string? Barcode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}
