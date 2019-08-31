using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Travel.BlaBlaCar
{
    public class Trip
    {
        public int Id { get; set; }
        [JsonProperty("links")]
        public Links Links { get; set; }
        [JsonProperty("departure_date")]
        public DateTime DepartureDate { get; set; }
        [JsonProperty("arrival_date")]
        public DateTime ArrivalDate { get; set; }
        [JsonProperty("departure_place")]
        public Place DeparturePlace { get; set; }
        [JsonProperty("arrival_place")]
        public Place ArrivalPlace { get; set; }
        [JsonProperty("price")]
        public Price Price { get; set; }
        [JsonProperty("seats_left")]
        public int SeatsLeft { get; set; }
        [JsonProperty("seats")]
        public int Seats { get; set; }
        [JsonProperty("duration")]
        public Duration Duration { get; set; }
        [JsonProperty("distance")]
        public Distance Distance { get; set; }
        [JsonProperty("car")]
        public Car Car { get; set; }
        [NotMapped]
        [JsonProperty("locations_to_display")]
        public string[] LocationsToDisplay { get; set; }
    }
}
