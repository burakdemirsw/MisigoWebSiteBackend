using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models
{
    public class ProductDetail_VM
    {

        public string? StockCode { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
        public List<Variant_VM>? Variations { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string? Brand { get; set; }
        public List<Photo_VM> PhotoUrl { get; set; }

    }

    public class Variant_VM
    {
        public string Quantity { get; set; }
        public string? Dimension { get; set; }
    }
    public class Photo_VM
    {
        public string Url { get; set; }
    }
}
