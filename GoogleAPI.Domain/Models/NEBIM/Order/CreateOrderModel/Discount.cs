namespace GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel
{
    public class Discount
    {
        public int DiscountTypeCode { get; set; }
        public int Value { get; set; }
        public string? DiscountReasonCode { get; set; }
        public bool IsPercentage { get; set; }
    }
}
