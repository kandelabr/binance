using Newtonsoft.Json;
using System.Collections.Generic;

namespace Binance.API.Models
{
    /// <summary>
    /// The ExchangeInfo response
    /// </summary>
    public class ExchangeInfoResponse
    {
        [JsonProperty("symbols")]
        public ExchangeInfo[] Symbols { get; set; }
    }

    /// <summary>
    /// The ExchangeInfo
    /// </summary>
    public class ExchangeInfo
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
        [JsonProperty("filters")]
        public List<Filter> Filters { get; set; }
    }

    public class Filter
    {
        [JsonProperty("filterType")]
        public string FilterType { get; set; }

        [JsonProperty("stepSize")]
        public string StepSize { get; set; }

        [JsonProperty("tickSize")]
        public string TickSize { get; set; }

    }
}
