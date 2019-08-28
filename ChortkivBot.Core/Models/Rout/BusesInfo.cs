using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class BusesInfo
    {
        [JsonProperty("dvs")]
        public Bus[] Buses { get; set; }
    }
}
