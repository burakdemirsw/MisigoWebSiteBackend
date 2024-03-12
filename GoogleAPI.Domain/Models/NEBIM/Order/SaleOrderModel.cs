namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class SaleOrderModel
    {
        public DateTime? OrderDate { get; set; }
        public string? OrderNumber { get; set; }
        public string? CurrAccCode { get; set; }
        public string? CurrAccDescription { get; set; }
        public string? SalespersonCode { get; set; }
        public int? Qty1 { get; set; }
        public int? Price { get; set; }
        public int? CollectedQty { get; set; }
        public int? Status { get; set; }
        public int? RemainingQty { get; set; }
        public string? Description { get; set; }
        public int? InvoiceStatus { get; set; }


    }
}
