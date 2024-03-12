namespace GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel
{
    public class OrdersViaInternetInfo
    {
        public string? SalesURL { get; set; }
        public int? PaymentTypeCode { get; set; }
        public string? PaymentTypeDescription { get; set; }
        public string? PaymentAgent { get; set; }
        public string? PaymentDate { get; set; }
        public string? SendDate { get; set; }
    }
}
