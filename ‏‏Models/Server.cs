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
        public string Id { get; set; }
        [JsonPropertyName("ServerUrl")]
        public string Url { get; set; }
    }
}
