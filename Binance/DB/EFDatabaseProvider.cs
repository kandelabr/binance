using Binance.API.Client;
using Binance.DB.Models;
using Binance.Handlers;
using Binance.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Binance.DB
{
    /// <summary>
    /// Db class
    /// </summary>
    public class EFDatabaseProvider : IDatabaseProvider
    {
        private readonly INotificationHandler _notificationHandler;
        private readonly ILogger<BinanceClient> _logger;

        public EFDatabaseProvider(INotificationHandler notificationHandler, ILogger<BinanceClient> logger)
        {
            _notificationHandler = notificationHandler;
            _logger = logger;
        }

        /// <summary>
        /// Upload batch of data into a DB
        /// </summary>
        public void HandleBatch(IList<PriceBatchData> batchData)
        {
            try
            {
                using (var context = new BinanceContext())
                {

                    var info = new Information()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Prices = batchData.Select(x => new Price()
                        {
                            Symbol = x.Symbol,
                            Amount = x.Price
                        }).ToList()
                    };

                    context.Informations.Add(info);
                    context.SaveChanges();
                }

                //using (var context = new BinanceContext())
                //{
                //    context.Database.ExecuteSqlRaw("exec [dbo].[ProcessBatch]");
                //}
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "DB update failed");
                Task.Run(() => _notificationHandler.SendEmail("DB update failed", ex.ToString()));
            }
        }
    }
}
