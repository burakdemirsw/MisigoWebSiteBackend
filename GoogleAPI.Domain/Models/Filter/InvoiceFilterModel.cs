namespace GoogleAPI.Domain.Models.Filter
{
    public class InvoiceFilterModel
    {
        public string? OrderNo { get; set; }
        public string? InvoiceType { get; set; }
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }
}
