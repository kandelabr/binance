using Binance.Models;
using System.Collections.Generic;

namespace Binance.DB
{
    /// <summary>
    /// Db interface
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        /// Upload batch of data into a DB
        /// </summary>
        void HandleBatch(IList<PriceBatchData> batchData);
    }
}
