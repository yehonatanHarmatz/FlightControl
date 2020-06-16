using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlan
    {
        public class Segment
        {
            [JsonPropertyName("longitude")]
            [JsonProperty("longitude")]
            [Range(-180, 180)]
            public Double Longitude { get; set; }
            [JsonPropertyName("latitude")]
            [JsonProperty("latitude")]
            [Range(-90, 90)]
            public Double Latitude { get; set; }
            [JsonPropertyName("timespan_seconds")]
            [JsonProperty("timespan_seconds")]
            public Int32 Time { get; set; }
            public bool IsValid()
            {
                return (Longitude <= 180)
                   && (Longitude >= -180) && (Latitude <= 90)
                   && (Latitude >= -90);
            }
        }
        public class InitialLocation
        {

            [JsonPropertyName("longitude")]
            [JsonProperty("longitude")]
            [Range(-180, 180)]

            public Double InitialLongitude { get; set; }
            [JsonPropertyName("latitude")]
            [JsonProperty("latitude")]
            [Range(-90, 90)]
            public Double InitialLatitude { get; set; }
            [JsonPropertyName("date_time")]
            [JsonProperty("date_time")]
            public DateTime Date { get; set; }
            public bool IsValid()
            {
                return (Date != null) && (InitialLongitude <= 180)
                   && (InitialLongitude >= -180) && (InitialLatitude <= 90)
                   && (InitialLatitude >= -90);
            }
        }
        public string Id { get; set; }
        [JsonPropertyName("company_name")]
        [JsonProperty("company_name")]
        public string CompanyName { get; set; }
        [JsonPropertyName("passengers")]
        [JsonProperty("passengers")]
        public int Passengers { get; set; }
        [JsonPropertyName("segments")]
        [JsonProperty("segments")]
        public List<Segment> Segments { get; set; }

        [JsonPropertyName("initial_location")]
        [JsonProperty("initial_location")]
        public InitialLocation InitLocation { get; set; }
        public bool IsValid()
        {
            bool isSegmentsValid = (Segments != null);
            if (isSegmentsValid)
            {
                foreach (Segment segment in Segments)
                {
                    isSegmentsValid &= segment.IsValid();
                }
            }
            return (CompanyName != null) && (InitLocation != null)
               && InitLocation.IsValid() && isSegmentsValid;
        }
        /*
         * convert the flight plan to flight
         */
        public Flight ConvertToFlight(DateTime time, Boolean isExternal)
        {
            if (time == null)
            {
                return null;
            }
            Flight flight = new Flight();
            flight.Id = Id;
            flight.CompanyName = CompanyName;
            flight.Passengers = Passengers;
            flight.Date = InitLocation.Date;
            flight.IsExternal = isExternal;
            CalculateLocation(time, flight);
            return flight;
        }
        /*
         * check if flight is "on air" at the time that recived.
         */
        public bool IsOnAir(DateTime time)
        {
            if (time == null)
            {
                return false;
            }
            TimeSpan timeSpan = time - InitLocation.Date;
            double timeInSec = timeSpan.TotalSeconds;
            if (timeInSec < 0)
            {
                return false;
            }
            double airTime = 0;
            foreach (Segment seg in Segments)
            {
                airTime += seg.Time;
            }
            if (timeInSec > airTime)
            {
                return false;
            }
            return true;
        }
        /*
         * calculate the location relative to specific time.
         * and change flight properties to the result.
         */
        private void CalculateLocation(DateTime time, Flight fligt)
        {
            TimeSpan timeSpan = time - InitLocation.Date;
            double timeInSec = timeSpan.TotalSeconds;
            double longitude = InitLocation.InitialLongitude;
            double latitude = InitLocation.InitialLatitude;
            double ratio;
            foreach (Segment seg in Segments)
            {
                if (timeInSec < seg.Time)
                {
                    ratio = timeInSec / seg.Time;
                    longitude = midpoint(longitude, seg.Longitude, ratio);
                    latitude = midpoint(latitude, seg.Latitude, ratio);
                    break;
                }
                timeInSec -= seg.Time;
                longitude = seg.Longitude;
                latitude = seg.Latitude;
            }
            fligt.Longitude = longitude;
            fligt.Latitude = latitude;
        }
        private double midpoint(double x1, double x2, double ratio)
        {
            return x1 + (x2 - x1) * ratio;
        }
    }
}
