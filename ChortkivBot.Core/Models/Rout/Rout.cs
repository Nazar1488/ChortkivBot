using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class Rout
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nm")]
        public string[] Names { get; set; }

        [JsonProperty("zns")]
        public Stop[] Stops { get; set; }
    }
}
