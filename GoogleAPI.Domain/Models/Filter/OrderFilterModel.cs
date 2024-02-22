namespace GoogleAPI.Domain.Models.Filter
{
    public class OrderFilterModel
    {
        public string? OrderNo { get; set; }
        public string? CurrAccCode { get; set; }
        public string? CustomerName { get; set; }
        public string? SellerCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
