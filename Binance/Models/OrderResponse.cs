namespace Binance.Models
{
    /// <summary>
    /// The request
    /// </summary>
    public class OrderResponse
    {
        /// <summary>
        /// The symbol
        /// </summary>
        /// <example>
        /// SHIBUSDT
        /// </example>
        public string Symbol { get; set; }

        /// <summary>
        /// Coin amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Coin price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Order Id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Error response
        /// </summary>
        public OrderErrorResponse ErrorResponse { get; set; }

        /// <summary>
        /// Error response class
        /// </summary>        
    }

    public class OrderErrorResponse
    {
        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public int ErrorCode { get; set; }
    }
}
