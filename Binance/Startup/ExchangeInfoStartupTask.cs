using Binance.API.Client;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    public class ExchangeInfoStartupTask : IStartupTask
    {
        private const string LOT_SIZE = "LOT_SIZE";
        private const string PRICE_FILTER = "PRICE_FILTER";
        private readonly IBinanceClient _binanceClient;

        public static Dictionary<string, byte> ExchangeInfoLotDictionary { get; private set; }
        public static Dictionary<string, byte> ExchangeInfoPriceDictionary { get; private set; }

        public ExchangeInfoStartupTask(IBinanceClient binanceClient)
        {
            _binanceClient = binanceClient;
        }

        public static bool ReadyToStart()
        {
            return ExchangeInfoLotDictionary.Any() && ExchangeInfoPriceDictionary.Any();
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var data = await _binanceClient.GetExchangeInfo().ConfigureAwait(false);
            if (data != null && data.Symbols?.Any() == true)
            {
                var usdtData = data.Symbols.Where(x => x.Symbol.Contains(Constants.CURRENCY));
                var lotData = usdtData.Select(y => new { Symbol = y.Symbol, LotSize = y.Filters.FirstOrDefault(z => z.FilterType == LOT_SIZE) })
                                .Where(t => t.LotSize != null)
                                .Select(e => new { e.Symbol, StepSize = GetScale(e.LotSize.StepSize) });
                var priceData = usdtData.Select(y => new { Symbol = y.Symbol, TickData = y.Filters.FirstOrDefault(z => z.FilterType == PRICE_FILTER) })
                                .Where(t => t.TickData != null)
                                .Select(e => new { e.Symbol, TickSize = GetScale(e.TickData.TickSize) });

                ExchangeInfoLotDictionary = lotData.ToDictionary(x => x.Symbol, x => x.StepSize);
                ExchangeInfoPriceDictionary = priceData.ToDictionary(x => x.Symbol, x => x.TickSize);
            }
            else
            {
                throw new System.Exception("Exchange info is NULL");
            }
        }

        private static byte GetScale(string stepSize)
        {
            double x = double.Parse(stepSize);
            int[] bits = decimal.GetBits((decimal)x);
            return (byte)((bits[3] >> 16) & 0x7F);
        }
    }
}
