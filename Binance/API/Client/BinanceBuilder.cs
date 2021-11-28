using Binance.API.Configuration;
using Binance.API.Models;
using Microsoft.Extensions.Options;
using System;

namespace Binance.API.Client
{
    public class BinanceBuilder : IBinanceBuilder
    {
        private readonly string _singKey;

        public BinanceBuilder(IOptions<KeysConfiguration> keysConfiguration)
        {
            _singKey = keysConfiguration.Value.Sign;
        }

        /// <summary>
        /// Builds signature
        /// </summary>
        /// <returns></returns>
        public string BuildSignature(string url)
        {
            return Helpers.SignatureHelper.Sign(url, _singKey);
        }

        /// <summary>
        /// Builds UNIX time stamp
        /// </summary>
        /// <returns></returns>
        public long BuildTimestamp()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Build buy request
        /// </summary>
        public OrderRequest BuildBuyOrderRequest(string symbol, decimal amount)
        {
            return new OrderRequest(symbol, amount)
            {
                OrderSide = OrderSide.BUY,
                OrderType = OrderType.MARKET
            };
        }

        /// <summary>
        /// Build sell limit request
        /// </summary>
        /// <remarks>
        /// Use it after BUY call
        /// </remarks>
        public OrderRequest BuildLimitSellOrderRequest(string symbol, decimal amount, decimal price)
        {
            return new OrderRequest(symbol, amount)
            {
                OrderSide = OrderSide.SELL,
                OrderType = OrderType.LIMIT,
                Price = price
            };
        }

        /// <summary>
        /// Build sell market request
        /// </summary>
        /// <remarks>
        /// Use it when the algorithm say so
        /// </remarks>
        public OrderRequest BuildMarketSellOrderRequest(string symbol, decimal amount)
        {
            return new OrderRequest(symbol, amount)
            {
                OrderSide = OrderSide.SELL,
                OrderType = OrderType.MARKET
            };
        }
    }
}
