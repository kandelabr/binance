using Newtonsoft.Json;

namespace Binance.API.Models
{
    public class ServerTimeResponse
    {
        [JsonProperty("serverTime")]
        public long ServerTime { get; set; }
    }
}
