namespace GooleAPI.Application.ViewModels
{
    public class VM_Create_Product
    {
        public string? ProductName { get; set; }
        public int? StockAmount { get; set; }

        public string? Barcode { get; set; }
        public string? StockCode { get; set; }
        public string? Dimention { get; set; } //yeni
        public int? PurchasePrice { get; set; } //yeni
        public int? SellingPrice { get; set; } //yeni
        public string? Url { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool? Blocked { get; set; }
        public bool? New { get; set; }

        public int? SubCategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? BrandId { get; set; }
    }
}
