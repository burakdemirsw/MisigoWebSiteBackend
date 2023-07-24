using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class ItemBillingModel
    {
        public string Itemcode { get; set; }
        public string ItemDescription { get; set; }
        public double Qty1 { get; set; }
        public double Loc_PriceVI { get; set; }
        public double Total { get; set; }
    }
}
