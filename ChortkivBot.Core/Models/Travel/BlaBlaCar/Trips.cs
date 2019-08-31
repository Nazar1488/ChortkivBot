using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Travel.BlaBlaCar
{
    public class Trips
    {
        [JsonProperty("trips")]
        public Trip[] AvailableTrips { get; set; }
    }
}
