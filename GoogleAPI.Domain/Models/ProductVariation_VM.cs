using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class ProductVariation_VM
    {
        public int Id { get; set; }
        public string StockCode { get; set; }
        public string Description { get; set; }
        public string Dimension { get; set; }
        public string Color { get; set; }
        public string Barcode { get; set; }
        public string CoverLetter { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int VATRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsNew { get; set; }
        public bool IsFreeCargo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
