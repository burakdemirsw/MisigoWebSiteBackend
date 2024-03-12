using GoogleAPI.Domain.Models.Cargo.Mng.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Cargo.Mng.Request
{
    public class CreateBarcode_MNG_Request
    {
        public string ReferenceId { get; set; }
        public string BillOfLandingId { get; set; }
        public int IsCOD { get; set; }
        public int CodAmount { get; set; }
        public int PackagingType { get; set; }
        public List<OrderPieceList_MNG> OrderPieceList { get; set; }
    }

}
