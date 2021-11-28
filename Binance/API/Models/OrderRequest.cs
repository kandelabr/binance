namespace Binance.API.Models
{
    public enum OrderType { LIMIT, MARKET }

    public enum OrderSide { BUY, SELL }

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
        /// Order type (BUY, SELL)
        /// </summary>
        public OrderSide OrderSide { private get; set; }

        /// <summary>
        /// String representation of a order type
        /// </summary>
        public string Side => OrderSide.ToString();

        /// <summary>
        /// Order type (LIMIT, MARKET)
        /// </summary>
        public OrderType OrderType { private get; set; }

        /// <summary>
        /// String representation of a order type
        /// </summary>
        public string Type => OrderType.ToString();

        /// <summary>
        /// USDT or Coin amount
        /// USDT => buy
        /// Coin => sell
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Price when selling on LIMIT
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Good till cancel
        /// </summary>
        public string TimeInForce => "GTC";

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

        public OrderRequest(string symbol, decimal amount)
        {
            Symbol = symbol;
            Amount = amount;
        }

        public string GetUnsecureParamsString()
        {
            var str = $"symbol={Symbol}&side={Side}&type={Type}&{GetOrderQuoteProperty()}={Amount}&recvWindow={RecvWindow}";
            str += AppandSellLimitPrice();
            str += $"&timestamp={Timestamp}";
            return str;
        }

        public string GetSecureParamsString()
        {
            var str = GetUnsecureParamsString();
            str += $"&signature={Signature}";
            return str;
        }

        private string GetOrderQuoteProperty()
        {
            if (OrderSide == OrderSide.BUY)
                return "quoteOrderQty";
            else
                return "quantity";
        }

        private string AppandSellLimitPrice()
        {
            if (OrderSide == OrderSide.SELL && OrderType == OrderType.LIMIT && Price != null)
                return $"&price={Price}&timeInForce={TimeInForce}";
            return string.Empty;
        }
    }
}
