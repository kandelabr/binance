namespace Binance.Models
{
    /// <summary>
    /// The request
    /// </summary>
    public class OrderRequest
    {
        /// <summary>
        /// The symbol
        /// </summary>
        /// <example>
        /// SHIBUSDT
        /// </example>
        public string Symbol { get; set; }

        /// <summary>
        /// USDT amount
        /// </summary>
        public decimal Amount { get; set; }


    }
}
