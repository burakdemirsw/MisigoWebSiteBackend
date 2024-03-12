namespace GoogleAPI.Domain.Models.NEBIM.Product
{
    public class ProductList_VM
    {
        public Guid? LineId { get; set; }

        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public string? WarehouseCode { get; set; }
        public string? PhotoUrl { get; set; }
        public string? ShelfNo { get; set; }
        public string? ItemCode { get; set; }
        public string? BatchCode { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? BrandDescription { get; set; }
        public int UD_Stock { get; set; }
        public int MD_Stock { get; set; }



    }


}
