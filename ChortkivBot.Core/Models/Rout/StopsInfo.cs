using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Rout
{
    public class StopsInfo
    {
        [JsonProperty("a1")]
        public StopInfo[] Stops { get; set; }
    }
}
