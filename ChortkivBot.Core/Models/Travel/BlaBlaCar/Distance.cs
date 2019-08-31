using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Travel.BlaBlaCar
{
    public class Distance
    {
        public int Id { get; set; }
        [JsonProperty("value")]
        public decimal Value { get; set; }
        [JsonProperty("unity")]
        public string Unity { get; set; }
    }
}
