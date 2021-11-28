using Binance.API.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Binance.API.Client
{
    /// <summary>
    /// Client interface
    /// </summary>
    public interface IBinanceInvoker
    {
        /// <summary>
        /// Buy crypto.
        /// </summary>
        Task<HttpResponseMessage> Order(OrderRequest request);

        /// <summary>
        /// Delete order.
        /// </summary>
        Task<HttpResponseMessage> Delete(DeleteOrderRequest request);

        /// <summary>
        /// Get server time
        /// </summary>
        /// <returns></returns>
        Task<HttpResponseMessage> GetServerTime();

        /// <summary>
        /// Get dictionary of prices
        /// </summary>
        /// <returns></returns>
        Task<HttpResponseMessage> GetPrices();

        /// <summary>
        /// Get exchange info of prices
        /// </summary>
        /// <returns></returns>
        Task<HttpResponseMessage> GetExchangeInfo();
    }
}
