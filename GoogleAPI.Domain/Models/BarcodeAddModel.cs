using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class BarcodeAddModel
    {
        public int Id { get; set; }
        public string Qr { get; set; }
        public string Barcode { get; set; }
        public string Party { get; set; }
        public string Shelf { get; set; }
        public int Quantity { get; set; }
        public string InvoiceNumber { get; set; }
    }



    public class BarcodeModel
    {

        public string Barcode { get; set; }
        public string UrunKodu { get; set; }
        public string ItemDescription { get; set; }
        public string Depo { get; set; }
        public string RafNo { get; set; }
        public string Parti { get; set; }
        public string AttributeCode { get; set; }
        public string PriceCurrency { get; set; }
        public decimal Price { get; set; }
        public int Miktar { get; set; }
        public string Resim { get; set; }
        public string Resim2 { get; set; }

    }
}
