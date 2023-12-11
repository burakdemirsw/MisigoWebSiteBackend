namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class CountConfirmModel
    {
        public int? ModelType { get; set; }
        public string? OfficeCode { get; set; }
        public string? StoreCode { get; set; }
        public string? WarehouseCode { get; set; }
        public int? CompanyCode { get; set; }
        public int? InnerProcessType { get; set; }
        public List<MyDataLine>? Lines { get; set; }
        public DateTime OperationDate { get; set; }
    }

}
