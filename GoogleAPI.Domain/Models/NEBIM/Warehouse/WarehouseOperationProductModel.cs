using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class WarehouseOperationProduct
    {
        public string Barcode { get; set; }
        public string Lot { get; set; }
        public string ShelfNumber { get; set; }
        public string Quantity { get; set; }
        public string Warehouse { get; set; }
        public string InnerNumber { get; set; }

    }
}
