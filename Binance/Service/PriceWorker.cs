using Binance.API.Client;
using Binance.DB;
using Binance.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Service
{
    public class PriceWorker : BackgroundService
    {
        private readonly IBinanceClient _binanceClient;
        private readonly ILogger<PriceWorker> _logger;
        private readonly IDatabaseProvider _databaseProvider;

        public PriceWorker(ILogger<PriceWorker> logger, IBinanceClient binanceClient, IDatabaseProvider databaseProvider)
        {
            _logger = logger;
            _binanceClient = binanceClient;
            _databaseProvider = databaseProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //await StartProcess(stoppingToken).ConfigureAwait(false);

                try
                {

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var data = await _binanceClient.GetPrices().ConfigureAwait(false);
                    var response = data.Select(x => new PriceBatchData() { Symbol = x.Values.ElementAt(0), Price = decimal.Parse(x.Values.ElementAt(1)) })
                                    .Where(y => y.Symbol.EndsWith("DOWNUSDT"))
                                    .Where(y => y.Symbol.EndsWith("UPUSDT"))
                                    .OrderBy(y => y.Symbol).ToList();

                    _databaseProvider.HandleBatch(response);

                    stopwatch.Stop();

                    await Task.Delay((int)(10000 - stopwatch.ElapsedMilliseconds > 0 ? 10000 - stopwatch.ElapsedMilliseconds : 0), stoppingToken);
                }
                catch (Exception ex) { _logger.LogError(ex, "Error in batch process"); }
            }
        }

        private async Task StartProcess(CancellationToken stoppingToken)
        {
            if (!ExchangeInfoStartupTask.ReadyToStart())
            {
                // Load all tasks from DI
                var startupTask = new ExchangeInfoStartupTask(_binanceClient);

                // Execute all the tasks
                await startupTask.ExecuteAsync(stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
