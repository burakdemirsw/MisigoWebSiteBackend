namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class WarehouseFormModel
    {
        public Guid? Id { get; set; }
        public string? ShelfNo { get; set; }
        public int? Quantity { get; set; }
        public string? BatchCode { get; set; }
        public string? Office { get; set; }
        public string? OfficeTo { get; set; }
        public string? Warehouse { get; set; }
        public string? WarehouseTo { get; set; }
        public string? OrderNo { get; set; }
        public string? Barcode { get; set; }
        //public string? ItemCode { get; set; }
        //public string? ColorCode { get; set; }
        //public string? ItemDim1Code { get; set; }

    }

}
