namespace GoogleAPI.Domain.Models.Cargo.Mng.Order
{
    public class Order_MNG
    {
        public string? ReferenceId { get; set; }
        public string? Barcode { get; set; }
        public string? BillOfLandingId { get; set; }
        public int IsCOD { get; set; }
        public int CodAmount { get; set; }
        public int ShipmentServiceType { get; set; }
        public int PackagingType { get; set; }
        public string? Content { get; set; }
        public int SmsPreference1 { get; set; }
        public int SmsPreference2 { get; set; }
        public int SmsPreference3 { get; set; }
        public int PaymentType { get; set; }
        public int DeliveryType { get; set; }
        public string? Description { get; set; }
        public string? MarketPlaceShortCode { get; set; }
        public string? MarketPlaceSaleCode { get; set; }

    }

}
