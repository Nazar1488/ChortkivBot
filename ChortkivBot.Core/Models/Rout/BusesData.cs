using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class BusesData
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("data")]
        public BusesInfo[] Buses { get; set; }
    }
}
