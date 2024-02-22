namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class WarehosueTransferListModel
    {
        public int? Quantity { get; set; }
        public string? OperationDate { get; set; }

        public string? OrderNumber { get; set; }
        public string? WarehouseCode { get; set; }
        public string? ToWarehouseCode { get; set; }

    }
}
