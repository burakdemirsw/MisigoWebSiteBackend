using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class InvoiceNumberModel
    {
        public string InvoiceNumber { get; set; }
    }

    public class RemainingProductsModel
    {
        public string ItemCode { get; set; }
        public double ProductsToBeCollected { get; set; }

    }
}
