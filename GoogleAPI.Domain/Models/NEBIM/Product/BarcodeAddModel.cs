namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class BarcodeAddModel
    {
        public int Id { get; set; }
        public string? Qr { get; set; }
        public string? Barcode { get; set; }
        public string? Party { get; set; }
        public string? Shelf { get; set; }
        public int Quantity { get; set; }
        public string? InvoiceNumber { get; set; }
    }



    public class BarcodeModel
    {

        public string? ItemCode { get; set; }
        public string? ItemDim1Code { get; set; }
        public string? ColorCode { get; set; }

        public string? Barcode { get; set; }
        public string? ItemDescription { get; set; }
        public string? Warehouse { get; set; }
        public string? shelfno { get; set; }
        public string? Party { get; set; }
        public string? AttributeCode { get; set; }
        public string? PriceCurrency { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Picture { get; set; }
        public string? Picture2 { get; set; }

    }
}
