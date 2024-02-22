using GoogleAPI.Domain.Models.NEBIM.Product;

namespace GoogleAPI.Domain.Models.NEBIM
{
    public class QrCode
    {
        public int Id { get; set; }
        public Guid  UniqueId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? BarcodeBase64 { get; set; }
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
        public string? BoxNo { get; set; }


    }
}
