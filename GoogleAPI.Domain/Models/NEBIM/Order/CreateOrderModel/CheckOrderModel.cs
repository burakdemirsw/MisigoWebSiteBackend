using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel
{
    public class CheckOrderModel
    {
        public Guid OrderHeaderID { get; set; }
        public string InternalDescription { get; set; }
        public string Customer { get; set; }
    }
}
