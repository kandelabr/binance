using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    /// <summary>
    /// Startup task interface
    /// </summary>
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
