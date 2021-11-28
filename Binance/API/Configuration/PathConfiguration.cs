namespace Binance.API.Configuration
{
    /// <summary>
    /// API configuration
    /// </summary>
    public class PathConfiguration
    {
        /// <summary>
        /// Orders (buy,sell) configuration path
        /// </summary>
        public string OrderPath { get; set; }

        /// <summary>
        /// Delete order
        /// </summary>
        public string DeletePath { get; set; }

        /// <summary>
        /// Server time configuration path
        /// </summary>
        public string ServerTimePath { get; set; }

        /// <summary>
        /// Prices configuration path
        /// </summary>
        public string PricesPath { get; set; }

        /// <summary>
        /// Exchange info path
        /// </summary>
        public string ExchangeInfoPath { get; set; }

        /// <summary>
        /// Base address
        /// </summary>
        public string BaseAdress { get; set; }
    }
}
