using Newtonsoft.Json;

namespace GoogleAPI.Domain.Models.NEBIM.Invoice
{
    public class VM_HttpConnectionRequest
    {
        [JsonProperty("SessionID")]
        public string SessionId { get; set; }
    }
}
