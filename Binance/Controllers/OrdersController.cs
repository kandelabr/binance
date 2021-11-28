using Binance.Handlers;
using Binance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Binance.Controllers
{
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersHandler _ordersHandler;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrdersHandler ordersHandler, ILogger<OrdersController> logger)
        {
            _ordersHandler = ordersHandler;
            _logger = logger;
        }

        /// <summary>
        /// Buy coin
        /// </summary>
        [HttpPost]
        [Route("/api/v1/binance/buy")]
        public async Task<IActionResult> Buy([FromBody] OrderRequest[] orderRequest)
        {
            try
            {
                var orderResponse = await _ordersHandler.Buy(orderRequest).ConfigureAwait(false);

                if (orderResponse != default)
                    return StatusCode(200, orderResponse);
                else
                    return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caught in BUY controller");
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Sell coin
        /// </summary>
        [HttpPost]
        [Route("/api/v1/binance/sell")]
        public async Task<IActionResult> Sell([FromBody] OrderRequest[] orderRequest)
        {
            try
            {
                var orderResponse = await _ordersHandler.Sell(orderRequest).ConfigureAwait(false);

                if (orderResponse != default)
                    return StatusCode(200, orderResponse);
                else
                    return StatusCode(500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caught in SELL controller");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
