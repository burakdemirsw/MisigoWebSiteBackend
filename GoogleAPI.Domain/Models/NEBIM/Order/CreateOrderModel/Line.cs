namespace GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel
{
    public class Line
    {
        //public string? ColorCode { get; set; } //x
        //public string? ItemDim1Code { get; set; }  //x
        public string? ItemCode { get; set; } //+
        //public string? BatchCode { get; set; } 
        public string? UsedBarcode { get; set; }//+
        public int? Qty1 { get; set; } //+
        //public int? LDisRate1 { get; set; } // ? 
        public double PriceVI { get; set; } //+
        public string? SalesPersonCode { get; set; }
    }
}
