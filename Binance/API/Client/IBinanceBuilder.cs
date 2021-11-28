using Binance.API.Models;

namespace Binance.API.Client
{
    /// <summary>
    /// Builder interface
    /// </summary>
    public interface IBinanceBuilder
    {
        /// <summary>
        /// Build UNIX time stamp in MS
        /// </summary>
        /// <returns></returns>
        long BuildTimestamp();

        /// <summary>
        /// Builds HmacSHA256 signature
        /// </summary>
        /// <returns></returns>
        string BuildSignature(string url);

        /// <summary>
        /// Build buy request
        /// </summary>
        public OrderRequest BuildBuyOrderRequest(string symbol, decimal amount);

        /// <summary>
        /// Build sell limit request
        /// </summary>
        /// <remarks>
        /// Use it after BUY call
        /// </remarks>
        public OrderRequest BuildLimitSellOrderRequest(string symbol, decimal amount, decimal price);

        /// <summary>
        /// Build sell market request
        /// </summary>
        /// <remarks>
        /// Use it when the algorithm say so
        /// </remarks>
        public OrderRequest BuildMarketSellOrderRequest(string symbol, decimal amount);
    }
}
