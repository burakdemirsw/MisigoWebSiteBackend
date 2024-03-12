using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.Cargo.Response
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<List<Root>>(myJsonResponse);

    public class CreateBarcode_MNG_Response
    {
        [JsonProperty("referenceId")]
        public string ReferenceId { get; set; }

        [JsonProperty("invoiceId")]
        public string InvoiceId { get; set; }

        [JsonProperty("shipmentId")]
        public string ShipmentId { get; set; }

        [JsonProperty("barcodes")]
        public List<Barcode_MNG> Barcodes { get; set; }

        [JsonProperty("referenceBarcodeOnError")]
        public string ReferenceBarcodeOnError { get; set; }
    }


    public class Barcode_MNG
    {
        [JsonProperty("pieceNumber")]
        public int? PieceNumber { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }
    }


}
