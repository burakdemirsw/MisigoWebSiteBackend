namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class ProductOfOrderModel
    {
        public string? Barcode { get; set; }
        public int? Quantity { get; set; }
        public string? PhotoUrl { get; set; }
        public string? ItemCode { get; set; }
        public string? ColorCode { get; set; }
        public string? ColorDescription { get; set; }
        public string? ItemDim1Code { get; set; }
        public string? RowNumber { get; set; }
        public string? ShelfNo { get; set; }
        public Guid PackageNo { get; set; }
        public int CountedQty { get; set; }
        public int CurrentQty { get; set; }
        public string? Description { get; set; }
        public Guid? LineId { get; set; }

    }


}
