namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class WarehosueOperationListModel
    {

        public Guid Id { get; set; }
        public DateTime OperationDate { get; set; }
        public string InnerNumber { get; set; }
        public string OfficeCode { get; set; }
        public string WarehouseCode { get; set; }
        public string ToOfficeCode { get; set; }
        public string ToWarehouseCode { get; set; }
        public int IsCompleted { get; set; }
    }
}
