namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class OrderBillingRequestModel
    {
        public string? OrderNo { get; set; }
        public bool InvoiceType { get; set; } //iade - normal 
        public int? InvoiceModel { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? Currency { get; set; }
        public int? TaxedOrTaxtFree { get; set; }

        public string? EInvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }

    }
}
