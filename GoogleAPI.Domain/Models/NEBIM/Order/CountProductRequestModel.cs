using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order
{
    public class CountProductRequestModel
    {
        public string Barcode { get; set; }
        public string ShelfNo { get; set; }

        public int Qty { get; set; }
        public string OrderNumber { get; set; }


    }
}
