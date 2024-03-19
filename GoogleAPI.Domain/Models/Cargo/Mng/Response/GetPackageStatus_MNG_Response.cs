using Newtonsoft.Json;

namespace GoogleAPI.Domain.Models.Cargo.Response
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(myJsonResponse);
    public class GetPackageStatus_MNG_Response
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("shipmentId")]
        public string ShipmentId { get; set; }

        [JsonProperty("shipmentSerialandNumber")]
        public string ShipmentSerialandNumber { get; set; }

        [JsonProperty("shipmentDateTime")]
        public string ShipmentDateTime { get; set; }

        [JsonProperty("shipmentStatus")]
        public object ShipmentStatus { get; set; }

        [JsonProperty("shipmentStatusCode")]
        public int? ShipmentStatusCode { get; set; }

        [JsonProperty("shipmentStatusExplanation")]
        public object ShipmentStatusExplanation { get; set; }

        [JsonProperty("statusDateTime")]
        public object StatusDateTime { get; set; }

        [JsonProperty("trackingUrl")]
        public string TrackingUrl { get; set; }

        [JsonProperty("isDelivered")]
        public int? IsDelivered { get; set; }

        [JsonProperty("deliveryDateTime")]
        public string DeliveryDateTime { get; set; }

        [JsonProperty("deliveryTo")]
        public string DeliveryTo { get; set; }

        [JsonProperty("retrieveShipmentId")]
        public object RetrieveShipmentId { get; set; }
    }




}
