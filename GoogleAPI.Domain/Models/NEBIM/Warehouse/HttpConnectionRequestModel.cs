using Newtonsoft.Json;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class HttpConnectionRequestModel
    {
        [JsonProperty("SessionID")]
        public string SessionId { get; set; }
    }
}

