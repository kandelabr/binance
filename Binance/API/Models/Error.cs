using Newtonsoft.Json;

namespace Binance.API.Models
{
    /// <summary>
    /// Error class
    /// </summary>
    public class Error
    {
        [JsonProperty("code")]
        public int Code { get; set; }


        [JsonProperty("msg")]
        public string Message { get; set; }

    }
}
