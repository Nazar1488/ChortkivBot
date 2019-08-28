using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class StopsData
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("data")]
        public StopsInfo Stops { get; set; }
    }
}
