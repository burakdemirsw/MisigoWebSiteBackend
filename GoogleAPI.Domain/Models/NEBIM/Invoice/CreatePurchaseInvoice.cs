namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class CreatePurchaseInvoice
    {
        public Guid? Id { get; set; }

        public string? ShelfNo { get; set; }
        public string? OfficeCode { get; set; }
        public string? WarehouseCode { get; set; }
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? CurrAccCode { get; set; }
        public string? OrderNumber { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? Currency { get; set; }
        public string? PhotoUrl { get; set; }
        public string? BatchCode { get; set; }
        public string? ItemCode { get; set; }
        public int Qty { get; set; }


    }
}
