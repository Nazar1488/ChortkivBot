using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class RoutesData
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("data")]
        public Rout[] Routes { get; set; }
    }
}
