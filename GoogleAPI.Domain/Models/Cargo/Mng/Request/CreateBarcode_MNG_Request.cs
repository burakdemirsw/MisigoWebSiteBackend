using GoogleAPI.Domain.Models.Cargo.Mng.Order;
using GoogleAPI.Domain.Models.Cargo.Mng.Response;
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
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }
        [JsonProperty("billOfLandingId")]
        public string BillOfLandingId { get; set; }
        [JsonProperty("isCOD")]
        public int IsCOD { get; set; }
        [JsonProperty("codAmount")]
        public int CodAmount { get; set; }
        [JsonProperty("packagingType")]
        public int PackagingType { get; set; }
        [JsonProperty("orderPieceList")]
        //public CreatePackage_MNG_RR Response { get; set; }
        public List<OrderPieceList_MNG> OrderPieceList { get; set; }
        
    }

}
