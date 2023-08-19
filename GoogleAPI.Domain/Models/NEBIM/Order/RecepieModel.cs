using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class RecepieModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentType { get; set; }
        public string EInvoiceNumber { get; set; }

        public string OrderNo { get; set; }
        public List<ProductRecepieModel> ProductRecepieModel { get; set; }
        public double TotalVAT { get; set; }
        public double TotalValue { get; set; }

        public string CustomerName { get; set; }

        public string OrderNoBase64String { get; set; }
    }
    public class ProductRecepieModel
    {
        //public int Id { get; set; }
        public string ItemCode { get; set; }
        public double Price { get; set; }

        public double Quantity { get; set; }
        //public double Amount { get; set; }
    }
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CustomerDescription { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int Qty1 { get; set; }
        public decimal Price { get; set; }
        public decimal Price2 { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentType { get; set; }
        public string TotalVat { get; set; }
    }
}
