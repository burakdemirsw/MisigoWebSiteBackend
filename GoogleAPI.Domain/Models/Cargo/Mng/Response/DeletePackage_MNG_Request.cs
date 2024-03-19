using Newtonsoft.Json;

namespace GoogleAPI.Domain.Models.Cargo.Mng.Response
{
    public class DeletePackage_MNG_Request
    {
        [JsonProperty("referenceId")]

        public string ReferenceId { get; set; }
        [JsonProperty("shipmentId")]

        public string ShipmentId { get; set; }

    }



}
