using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class ProductCard_VM
    {
        public string StockCode { get; set; }
        public string? Description { get; set; }
        public string? CoverLetter { get; set; }
        public string? PhotoUrl { get; set; }

        public string? Brand { get; set; }
        public decimal? NormalPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int? VATRate { get; set; }
        public int? TotalStockAmount { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsFreeCargo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
