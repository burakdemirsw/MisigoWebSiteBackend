namespace GoogleAPI.Domain.Models.NEBIM
{
    public class BarcodeModel_A
    {
        public string? ItemArea { get; set; }
        public string? RefArea { get; set; }
        public string? LotArea { get; set; }
        public int? QtyArea { get; set; }
        public string? AddressArea { get; set; }
        public string? BoxArea { get; set; }

        public DateTime? DateArea { get; set; }
        public string? TopBarcodeJSON { get; set; }
        public string? ProductBarcodeJSON { get; set; }
        public string? ItemCode { get; set; }
        public string? Barcode { get; set; }


    }

    public class BarcodeModelResponse
    {
        public string? Page { get; set; }



    }
    public class BarcodeModel_B
    {
    }
    public class BarcodeModel_C
    {
    }
}
