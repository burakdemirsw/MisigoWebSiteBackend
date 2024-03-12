using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class OrderDetail_Model
    {
        public int TotalPrice { get; set; }
        public string? OrderDate { get; set; } 
        public string? SalespersonCode { get; set; }
        public string? Description { get; set; }
        public string? OrderNumber { get; set; }
        public string? CurrAccCode { get; set; }
        public string? Phone { get; set; }
        public string? Mail { get; set; }
        public string? Customer { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public string? Products { get; set; }
    }

    public class OrderDetail
    {
        public int TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string? SalespersonCode { get; set; }
        public string? Description { get; set; }
        public string? OrderNumber { get; set; }

        public string? CurrAccCode { get; set; }
        public string? Phone { get; set; }
        public string? Mail { get; set; }
        public string? Customer { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public List<BasketProductSummary>? Products { get; set; }
    }
    public class BasketProductSummary
    {
        public string? ItemCode { get; set; }
        public string? Barcode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
    }

}
