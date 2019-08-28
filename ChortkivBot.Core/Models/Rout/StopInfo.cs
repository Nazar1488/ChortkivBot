using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class StopInfo
    {
        [JsonProperty("dId")]
        public int BusId { get; set; }

        [JsonProperty("rId")]
        public int RoutId { get; set; }

        [JsonProperty("t")]
        public int Time { get; set; }
    }
}
