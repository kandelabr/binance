namespace Binance.API.Models
{
    /// <summary>
    /// The request
    /// </summary>
    public class DeleteOrderRequest
    {
        /// <summary>
        /// The symbol
        /// </summary>
        /// <example>
        /// SHIBUSDT
        /// </example>
        public string Symbol { get; set; }

        /// <summary>
        /// The time stamp
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Hardcode to 55 secs
        /// </summary>
        public long RecvWindow => 55000;

        /// <summary>
        /// The HmacSHA256 signature
        /// </summary>
        public string Signature { get; set; }

        public string GetUnsecureParamsString()
        {
            return $"symbol={Symbol}&recvWindow={RecvWindow}&timestamp={Timestamp}";
        }

        public string GetSecureParamsString()
        {
            return $"symbol={Symbol}&recvWindow={RecvWindow}&timestamp={Timestamp}&signature={Signature}";
        }

        public DeleteOrderRequest(string symbol)
        {
            Symbol = symbol;
        }
    }
}
