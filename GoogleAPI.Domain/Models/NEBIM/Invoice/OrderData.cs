using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class OrderData
    {
        public string? InternalDescription { get; set; } //solo
        public string? OrderNumber { get; set; } //solo

        public string? OrderNo { get; set; } //solo
        public string? EInvoicenumber { get; set; } //solo
        public string? DocCurrencyCode { get; set; } //solo

        public int TaxTypeCode { get; set; } //solo


        public Guid OrderHeaderID { get; set; } //solo
        public string? Lines { get; set; }

        public string? Products { get; set; }
        public string? CurrAccCode { get; set; }
        public string? Payments { get; set; }
        public string? CustomerTaxInfo { get; set; }
        public string? EMailAddress { get; set; }
        public string? Description { get; set; }
        public string? BillingAddress { get; set; }
        public string? ShipmentAddress { get; set; }
        public string? InvoiceData { get; set; }
        public string? ShipmentMethodCode { get; set; }
        public string? DeliveryCompanyCode { get; set; }
        public decimal? CompanyCode { get; set; }
        public bool? IsSalesViaInternet { get; set; }
        public Guid BillingPostalAddressID { get; set; }
        public Guid ShippingPostalAddressID { get; set; }
        public string? OfficeCode { get; set; }
        public string? StoreCode { get; set; }
        public string? WareHouseCode { get; set; }
        public string? PostalAddress { get; set; }
        public DateTime? OrderDate { get; set; }
    }
    public class EInvoiceModel
    {
        public int Id { get; set; }
        public string? OrderNumber { get; set; }
        public string? OrderNo { get; set; } //solo

        public string? EInvoiceNumber { get; set; }

        public string? UnofficialInvoiceString { get; set; }

        public DateTime InvoiceDatetime { get; set; }
    }

    public class OrderErrorModel
    {
        public int ModelType { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? ErrorSource { get; set; }
        public int StatusCode { get; set; }
    }

}
