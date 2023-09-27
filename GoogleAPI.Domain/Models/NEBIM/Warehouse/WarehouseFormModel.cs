using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class WarehouseFormModel
    {
        public int Id { get; set; }
        public string? ShelfNo { get; set; }
        public int? Inventory { get; set; }
        public string? Party { get; set; }
        public string? Office { get; set; }
        public string? OfficeTo { get; set; }
        public string? Warehouse { get; set; }
        public string? WarehouseTo { get; set; }
        public string? ItemCode { get; set; }
        public string? ColorCode { get; set; }
        public string? Barcode { get; set; }
        public string? ItemDim1Code { get; set; }

    }

}
