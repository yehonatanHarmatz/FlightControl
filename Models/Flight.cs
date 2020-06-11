using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlightControlWeb.Models
{
    public class Flight
    {
        [JsonPropertyName("flight_id")]
        [JsonProperty("flight_id")]
        public string Id { get; set; }
        [JsonPropertyName("longitude")]
        [JsonProperty("longitude")]
        [Range(-180,180)]
        public Double Longitude { get; set; }
        [JsonPropertyName("latitude")]
        [JsonProperty("latitude")]
        [Range(-90, 90)]
        public Double Latitude { get; set; }
        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
        [JsonPropertyName("passengers")]
        [JsonProperty("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("date_time")]
        [JsonProperty("date_time")]
        public DateTime Date { get; set; }
        [JsonPropertyName("is_external")]
        [JsonProperty("is_external")]
        public bool IsExternal { get; set; }
        /*
         * check if valid plan
         */
        public bool IsValid()
        {
            return (CompanyName != null) && (Id != null) && (Longitude <= 180)
                   && (Longitude >= -180) && (Latitude <= 90)
                   && (Latitude >= -90);
        }
    }
}
