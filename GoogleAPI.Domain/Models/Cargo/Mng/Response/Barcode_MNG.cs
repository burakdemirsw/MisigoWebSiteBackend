using Newtonsoft.Json;

namespace GoogleAPI.Domain.Models.Cargo.Response
{
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
