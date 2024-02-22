namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class CountListModel
    {
        public DateTime? LastUpdateDate { get; set; }
        public int? TotalProduct { get; set; }
        public string? OrderNo { get; set; } //IsReturn,SalesPersonCode,TaxTypeCode
        public bool? IsReturn { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? TaxTypeCode { get; set; }
    }

}
