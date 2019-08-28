using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class Point
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}
