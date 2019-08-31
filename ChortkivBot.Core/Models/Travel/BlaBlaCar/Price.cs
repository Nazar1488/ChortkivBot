using Newtonsoft.Json;

namespace ChortkivBot.Core.Models.Travel.BlaBlaCar
{
    public class Price
    {
        public int Id { get; set; }
        [JsonProperty("value")]
        public decimal Value { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("string_value")]
        public string StringValue { get; set; }
        [JsonProperty("price_color")]
        public string PriceColor { get; set; }
    }
}
