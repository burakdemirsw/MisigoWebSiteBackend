using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Product;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Payment
{
    public class Payment_CM
    {
        public string OrderNo { get; set; }
        public string TotalValue { get; set; }
        public CustomerList_VM User { get; set; }
        public List<ProductList_VM> BasketItems { get; set; }
        public CustomerAddress_VM Address { get; set; }

    }
    public class Payment_CR
    {
        public string PageUrl { get; set; }
    }
    public class Payment : BaseEntity
    {
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Boolean Status { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal PaymentValue { get; set; }
        public string? PaymentToken { get; set; }
        public string? ExceptionCode { get; set; }
        public string? ExceptionDescription { get; set; }
        public string? OrderNo { get; set; }
        public string? ConversationId { get; set; }


    }
    public class Product_PayTR
    {
        public string? Description { get; set; }
        public string? NormalPrice { get; set; }
        public int? StockAmount { get; set; }

    }


}
