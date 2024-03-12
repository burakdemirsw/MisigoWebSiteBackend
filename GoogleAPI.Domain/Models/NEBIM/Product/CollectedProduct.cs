namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class CollectedProduct
    {
        public Guid? Id { get; set; }

        public string? PhotoUrl { get; set; }
        public string? ShelfNo { get; set; }
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? ItemCode { get; set; }
        public string? BatchCode { get; set; }
        public string? LineId { get; set; }
        public int? AvailableQty { get; set; }
    }

    public class CountedProduct
    {
        public Guid? Id { get; set; }

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
        public int? Quantity { get; set; }
        public string? TargetShelf { get; set; }
        public int? TransferQuantity { get; set; }
        public string? Barcode { get; set; }
        public int DrawerCount { get; set; }
    }


}
