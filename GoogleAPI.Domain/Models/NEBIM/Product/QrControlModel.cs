namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class QrControlModel
    {
        public string? Barcode { get; set; }
        public string? BatchCode { get; set; }
        public string? AttributeCode { get; set; }
        public string? PriceCurrency { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }
    }
    public class QrControlCommandModel
    {
        public string? Qr { get; set; }

    }
}
