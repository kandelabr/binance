namespace Binance.Models
{
    /// <summary>
    /// Price data
    /// </summary>
    public class PriceBatchData
    {
        /// <summary>
        /// The symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// The amount
        /// </summary>
        public decimal Price { get; set; }
    }
}
