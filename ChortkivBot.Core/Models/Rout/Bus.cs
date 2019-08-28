using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class Bus
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("gNb")]
        public string Number { get; set; }

        [JsonProperty("loc")]
        public Point Location { get; set; }

        [JsonProperty("spd")]
        public int Speed { get; set; }
    }
}
