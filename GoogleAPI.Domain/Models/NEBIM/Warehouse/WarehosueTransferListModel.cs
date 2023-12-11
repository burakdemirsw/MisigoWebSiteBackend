using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class WarehosueTransferListModel
    {
        public  int? Quantity { get; set; }
        public DateTime? OperationDate { get; set; }

        public string? OrderNumber { get; set; }
        public string? WarehouseCode { get; set; }
        public string? ToWarehouseCode { get; set; }

    }
}
