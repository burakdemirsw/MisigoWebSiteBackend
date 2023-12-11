using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class CollectedProduct
    {
        public string? PhotoUrl { get; set; }
        public string? ShelfNo { get; set; }
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? ItemCode { get; set; }
    }

    public class CountedProduct
    {
        public string? PhotoUrl { get; set; }
        public string? ShelfNo { get; set; }
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? BatchCode { get; set; }
        public string? ItemCode { get; set; }
    


    }
    public class CountedProduct2
    {
        public string? PhotoUrl { get; set; }
        public string? ShelfNo { get; set; }
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? BatchCode { get; set; }
        public string? WarehouseCode { get; set; }
        public string? OfficeCode { get; set; }
        public string? CurrAccCode { get; set; }

    }

    public class TransferRequestListModel
    {
        public string? ItemCode { get; set; }
        public string? ShelfNo { get; set; }
        public  int? Quantity { get; set; }
        public string? TargetShelf { get; set; }
        public int? TransferQuantity { get; set; }
    }


}
