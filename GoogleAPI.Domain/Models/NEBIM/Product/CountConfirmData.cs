namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class CountConfirmData
    {
        public string? OfficeCode { get; set; }
        public int? ModelType { get; set; }
        public string? StoreCode { get; set; }
        public string? WarehouseCode { get; set; }
        public int? CompanyCode { get; set; }
        public int? InnerProcessType { get; set; }
        public string? Lines { get; set; }
        public DateTime OperationDate { get; set; }
    }


    public class MyDataLine
    {
        public string UsedBarcode { get; set; }
        public string BatchCode { get; set; }
        public double Qty1 { get; set; }
    }

}
