namespace GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel
{
    public class Payment
    {
        public string? PaymentType { get; set; }
        public string? Code { get; set; }
        public string? CreditCardTypeCode { get; set; }
        public int? InstallmentCount { get; set; }
        public string? CurrencyCode { get; set; }
        public double Amount { get; set; }
    }
}
