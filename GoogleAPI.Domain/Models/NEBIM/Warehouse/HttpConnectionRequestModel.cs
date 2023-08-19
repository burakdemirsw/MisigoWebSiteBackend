using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Domain.Models.NEBIM.Warehouse
{
    public class HttpConnectionRequestModel
    {
        [JsonProperty("SessionID")]
        public string SessionId { get; set; }
    }
}

