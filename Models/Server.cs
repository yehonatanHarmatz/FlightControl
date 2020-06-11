using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Server
    {
        [JsonPropertyName("ServerId")]
        [JsonProperty("ServerId")]
        public string Id { get; set; }
        [JsonPropertyName("ServerUrl")]
        [JsonProperty("ServerUrl")]
        public string Url { get; set; }
        /*
         * check if valid.
         */
        public bool IsValid()
        {
            return Id != null && Url != null;
        }
    }
}
