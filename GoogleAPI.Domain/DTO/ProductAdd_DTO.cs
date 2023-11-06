using GoogleAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.DTO
{
    public class ProductAdd_DTO
    {
        public string StockCode { get; set; }

        public string Description { get; set; }

        public int MainCategoryId { get; set; }

        public List<int> ColorIdList { get; set; }

        public List<int> ItemDimList { get; set; }

        public int BrandId { get; set; }

        public string? Explanation { get; set; }

        public string? CoverLetter { get; set; }

        public int? StockAmount { get; set; } 

        public decimal? NormalPrice { get; set; } 

        public decimal? PurchasePrice { get; set; }

        public decimal? DiscountedPrice { get; set; } 

        public int? VatRate { get; set; }

        public bool? IsActive { get; set; } 

        public bool? IsNew { get; set; } 

        public bool? IsFreeCargo { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
