using Binance.Models;
using System.Threading.Tasks;

namespace Binance.Handlers
{
    /// <summary>
    /// Orders interface
    /// </summary>
    public interface IOrdersHandler
    {
        /// <summary>
        /// Buys array of coin requests
        /// </summary>
        Task<OrderResponse[]> Buy(OrderRequest[] orderRequest);

        /// <summary>
        /// Sells array of coin requests
        /// </summary>
        Task<OrderResponse[]> Sell(OrderRequest[] orderRequest);
    }
}
