using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [JsonPropertyName("flight_id")]
        public string Id { get; set; }
        [JsonPropertyName("longitude")]
        public Double Longtitude { get; set; }
        [JsonPropertyName("latitude")]
        public Double Latitude { get; set; }
        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }
        [JsonPropertyName("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("date_time")]
        public DateTime Date { get; set; }
        [JsonPropertyName("is_external")]
        public bool IsExternal { get; set; }

    }
}
