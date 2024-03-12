using GoogleAPI.Domain.Models.NEBIM.Product;

namespace GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel
{
    public class NebimOrder
    {
        
        public int? ModelType { get; set; }
        public string? CustomerCode { get; set; } //CurrAccCode
        public string? InternalDescription { get; set; } //RANDOM 10 CHAR
        public string? OrderDate { get; set; } //NOW 
        public string? OfficeCode { get; set; } // OFİS DEPO SEÇİMİ EN ÜSTTE EKLENCEK
        //public string? StoreCode { get; set; } // OFİS DEPO SEÇİMİ EN ÜSTTE EKLENCEK
        public string? WarehouseCode { get; set; }  // OFİS DEPO SEÇİMİ EN ÜSTTE EKLENCEK
        public string? DeliveryCompanyCode { get; set; } // KRG
        public int? ShipmentMethodCode { get; set; } //?
        public int? PosTerminalID { get; set; } // ??
        public bool IsCompleted { get; set; } //true
        public bool IsSalesViaInternet { get; set; } //true
        public string? DocumentNumber { get; set; } //MSG_1238 
        public string? Description { get; set; }//RANDOM 10 CHAR
        public List<Line>? Lines { get; set; } //ÜRÜNLER
        public OrdersViaInternetInfo? OrdersViaInternetInfo { get; set; } //newle gönder 
        //public List<Discount>? Discounts { get; set; } //new le gönder
        //public List<Payment>? Payments { get; set; } //seçili olan ödeme tipine göre gönder
    }
    public class ClientOrder_DTO
    {
        public ClientOrder? ClientOrder { get; set; }
        public List<ClientOrderBasketItem>? ClientOrderBasketItems { get; set; } 
    }
    public class ClientOrder
    {
        public Guid Id { get; set; }
        public string? CustomerCode { get; set; }

        public Guid ShippingPostalAddressId { get; set; }
        public string? OrderNo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? PaymentDescription { get; set; }
    }
    public class ClientOrderBasketItem
    {
        public Guid? Id { get; set; }
        public Guid OrderId { get; set; }

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
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
