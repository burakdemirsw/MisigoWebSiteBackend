namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class OrderDataModel
    {
        public string? InternalDescription { get; set; }
        public string? OrderNumber { get; set; }
        public string? OrderNo { get; set; } //solo
        public string? EInvoicenumber { get; set; } //solo
        public string? DocCurrencyCode { get; set; } //solo

        public int TaxTypeCode { get; set; } //solo

        public Guid OrderHeaderID { get; set; }
        public List<OrderLine>? Lines { get; set; }
        public List<Payment>? Payments { get; set; }
        public string? CurrAccCode { get; set; }
        public List<CustomerTaxInfo>? CustomerTaxInfo { get; set; }
        public string? EMailAddress { get; set; }
        public string? Description { get; set; }
        public List<BaseAddress>? BillingAddress { get; set; }
        public List<BaseAddress>? ShipmentAddress { get; set; }
        public Guid BillingPostalAddressID { get; set; }
        public Guid ShippingPostalAddressID { get; set; }
        public string? DeliveryCompanyCode { get; set; }
        public string? CompanyCode { get; set; }
        public string? IsSalesViaInternet { get; set; }
        public string? OfficeCode { get; set; }
        public string? StoreCode { get; set; }
        public string? WareHouseCode { get; set; }
        public List<PostalAddress>? PostalAddress { get; set; }
        public string? ShipmentMethodCode { get; set; }
        public List<Invoice>? InvoiceData { get; set; }
        public List<Product>? Products { get; set; }
    }
    public class CustomerTaxInfo
    {
        public string? CompanyName { get; set; }
        public string? TaxOfficeCode { get; set; }
        public string? TaxID { get; set; }
        public string? TaxOfficeDescription { get; set; }
    }
    public class BaseAddress
    {
        public int AddressId { get; set; }
        public string? Address { get; set; }
        public string? Place { get; set; }
    }
    public class PostalAddress
    {
        public int AddressId { get; set; }
        public string? Address { get; set; }
        public string? CityCode { get; set; }
        public string? CountryCode { get; set; }
        public string? StateCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? CompanyName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IdentityNum { get; set; }
        public string? TaxNumber { get; set; }
        public string? TaxOfficeCode { get; set; }
        public string? ZipCode { get; set; }
    }
    public class Invoice
    {
        public Guid InvoiceHeaderID { get; set; }
        public string? InvoiceTypeCode { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? EInvoiceNumber { get; set; }
    }
    public class Product
    {
        public string? OrderNumber { get; set; }
        public string? Siparis_Numarasi { get; set; }
        public string? Tarih { get; set; }
        public string? Musteri_Kodu { get; set; }
        public string? Musteri_Adi { get; set; }
        public string? Kargo { get; set; }
        public string? Urun_Kodu { get; set; }
        public string? Urun_Adi { get; set; }
        public string? Renk { get; set; }
        public decimal? Miktar { get; set; }
        public string? Barkod { get; set; }
    }
    public class OrderLine
    {
        public string? ItemCode { get; set; }
        public string? UsedBarcode { get; set; }
        public string? SalesPersonCode { get; set; }
        public string? BatchCode { get; set; }
        public List<ITAttribute>? ITAttributes { get; set; }
        public decimal LDisRate1 { get; set; }
        public int VatRate { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int Qty1 { get; set; }
        public string? DocCurrencyCode { get; set; }
        public string? CurrencyCode { get; set; }

    }
    public class OrderLineBP
    {
        // public Guid OrderLineID { get; set; }
        public string? UsedBarcode { get; set; }

        //public string SalesPersonCode { get; set; }
        public string? BatchCode { get; set; }

        public List<ITAttribute>? ITAttributes { get; set; }
        public decimal LDisRate1 { get; set; }
        public int VatRate { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int Qty1 { get; set; }
    }

    public class OrderLineBP2
    {
        // public Guid OrderLineID { get; set; }
        public string? UsedBarcode { get; set; }

        //public string SalesPersonCode { get; set; }
        public string? BatchCode { get; set; }

        public List<ITAttribute>? ITAttributes { get; set; }
        public decimal LDisRate1 { get; set; }
        public int VatRate { get; set; }
        public decimal PriceVI { get; set; }
        public decimal AmountVI { get; set; }
        public string? SalesPersonCode { get; set; }
        public int Qty1 { get; set; }
    }
    public class OrderLineBP3
    {
        public string? UsedBarcode { get; set; }
        public string? ItemCode { get; set; }
        //public string? SalesPersonCode { get; set; }
        public string? BatchCode { get; set; }
        public List<ITAttribute>? ITAttributes { get; set; }
        public decimal LDisRate1 { get; set; }
        public int VatRate { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int Qty1 { get; set; }
        public string? DocCurrencyCode { get; set; }
        public string? CurrencyCode { get; set; }
    }
    public class Payment
    {
        public int PaymentType { get; set; }
        public string? Code { get; set; }
        public string? CreditCardTypeCode { get; set; }
        public int InstallmentCount { get; set; }
        public string? CurrencyCode { get; set; }
        public decimal Amount { get; set; }
    }

    public class ITAttribute
    {
        public string? AttributeCode { get; set; }
        public int AttributeTypeCode { get; set; }
    }
}
