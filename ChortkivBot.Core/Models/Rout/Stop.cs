using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class Stop
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nm")]
        public string[] Names { get; set; }

        [JsonProperty("pt")]
        public Point Point { get; set; }
    }
}
