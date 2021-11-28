using Binance.Models;

namespace Binance.Helpers
{
    /// <summary>
    /// Orders helper class
    /// </summary>
    public static class OrdersHandlerHelper
    {
        /// <summary>
        /// Returns error object
        /// </summary>
        public static OrderResponse GetErrorResponse(System.Net.HttpStatusCode code, string message, string symbol)
        {
            return new OrderResponse()
            {
                Symbol = symbol,
                ErrorResponse = new OrderErrorResponse()
                {
                    ErrorCode = (int)code,
                    Message = message
                }
            };
        }
    }
}
