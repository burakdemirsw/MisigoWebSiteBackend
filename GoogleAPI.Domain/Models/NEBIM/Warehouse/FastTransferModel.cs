using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class FastTransferModel
    {
        
        public Guid Id { get; set; }
        public string? OperationId { get; set; }
        //public string? PhotoUrl { get; set; } //yeni
        //public string? ItemCode { get; set; } //yeni

        public string? Barcode { get; set; }
        public string? ShelfNo { get; set; }
        public int? Quantity { get; set; }
        public string? BatchCode { get; set; }
        public string? TargetShelfNo { get; set; }
        public string? WarehouseCode { get; set; }

    }
}
