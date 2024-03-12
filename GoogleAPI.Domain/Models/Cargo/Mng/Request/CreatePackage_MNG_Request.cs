using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleAPI.Domain.Models.Cargo.Mng.Order;


namespace GoogleAPI.Domain.Models.Cargo.Mng.Request
{
    public class CreatePackage_MNG_Request
    {
        public Order_MNG Order { get; set; }
        public List<OrderPieceList_MNG> OrderPieceList { get; set; }
        //public Shipper_MNG Shipper { get; set; }
        public Recipient_MNG Recipient { get; set; }
    }

}
