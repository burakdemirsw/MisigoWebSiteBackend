using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleAPI.Domain.Entities.Common;

namespace GoogleAPI.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string StockCode { get; set; }
        public string Description { get; set; }




        //public ICollection<ProductSubCategory> SubCategories { get; set; } // Ürünün birden çok Alt Kategori ilişkisini temsil eden navigation property
        public ICollection<ProductPhoto> Photos { get; set; } // Ürünün birden çok Alt Kategori ilişkisini temsil eden 


        [ForeignKey(nameof(Dimension))]
        public int DimensionId { get; set; } // Beden için referans anahtar
        public Dimension Dimension { get; set; } // Beden ile ilişkiyi temsil eden navigation property


        [ForeignKey(nameof(Color))]
        public int ColorId { get; set; } // Renk için referans anahtar ++
        public Color Color { get; set; } // Renk ile ilişkiyi temsil eden navigation property

        [ForeignKey(nameof(MainCategory))]
        public int MainCategoryId { get; set; } // Ana Kategoriye referans veren anahtar++
        public MainCategory MainCategory { get; set; } // Ana Kategori ile ilişkiyi temsil eden navigation property


        [ForeignKey(nameof(Brand))]
        public int BrandId { get; set; } // Ana Kategoriye referans veren anahtar++
        public Brand Brand { get; set; } // Ana Kategori ile ilişkiyi temsil eden navigation property



        public string? Barcode { get; set; }
        public string? Explanation { get; set; }
        public string? CoverLetter { get; set; }
        public decimal? NormalPrice { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int? StockAmount { get; set; }

        public int? VATRate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsFreeCargo { get; set; }


    }
}
