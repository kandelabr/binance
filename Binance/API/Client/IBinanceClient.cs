using Binance.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binance.API.Client
{
    /// <summary>
    /// Client interface
    /// </summary>
    public interface IBinanceClient
    {
        /// <summary>
        /// Buy crypto.
        /// </summary>
        /// <remarks>
        /// In case of SHIBUSDT we will buy SHIB, sell USDT
        /// </remarks>
        Task<OrderResponse> Buy(OrderRequest request);

        /// <summary>
        /// Sell crypto.
        /// </summary>
        /// <remarks>
        /// In case of SHIBUSDT we will sell SHIB, buy USDT
        /// </remarks>
        Task<OrderResponse> Sell(OrderRequest request);

        /// <summary>
        /// Delete order.
        /// </summary>
        Task<DeleteOrderResponse> Delete(DeleteOrderRequest request);

        /// <summary>
        /// Get server time
        /// </summary>
        /// <returns></returns>
        Task<ServerTimeResponse> GetServerTime();

        /// <summary>
        /// Get dictionary of prices
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, string>[]> GetPrices();

        /// <summary>
        /// Get exchange info of prices
        /// </summary>
        /// <returns></returns>
        Task<ExchangeInfoResponse> GetExchangeInfo();
    }
}
