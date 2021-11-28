using Binance.API.Client;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Binance.Controllers
{
    public class PriceController : ControllerBase
    {
        private readonly IBinanceClient _binanceClient;

        public PriceController(IBinanceClient binanceClient)
        {
            _binanceClient = binanceClient;
        }

        /// <summary>
        /// Get the JWT token containing user data
        /// </summary>
        /// <response code="200">Success</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("/api/v1/binance/get-prices/")]
        public async Task<IActionResult> GetPrices()
        {
            var response = await _binanceClient.GetPrices().ConfigureAwait(false);
            var dict = response.Select(x => new { Symbol = x.Values.ElementAt(0), Price = decimal.Parse(x.Values.ElementAt(1)) })
                .Where(y => y.Symbol.EndsWith("USDT"))
                .Where(y => !y.Symbol.Contains("DOWN"))
                .OrderBy(y => y.Symbol);
            return StatusCode(200, dict);
        }
    }
}
